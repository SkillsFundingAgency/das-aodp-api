using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class RolloverCandidateReconciler(
    IApplicationDbContext context,
    IRolloverFundingEligibilityRepository eligibilityRepository,
    ISystemClockProvider clock)
    : IRolloverCandidateReconciler
{
    private const string FundingNoLongerApplicableReason =
        "The source funding is no longer applicable for rollover.";

    public async Task<FundingReconciliationResult> ReconcileAsync(
        IReadOnlyCollection<FundingChangeKey> keys,
        CancellationToken cancellationToken)
    {
        var eligibilityResults = await eligibilityRepository.GetAsync(keys, cancellationToken);
        if (eligibilityResults.Count == 0)
        {
            return new FundingReconciliationResult(0, 0, 0, 0, 0, []);
        }

        var sourceQualificationIds = eligibilityResults
            .Select(x => x.Key.SourceQualificationId)
            .Distinct()
            .ToList();
        var fundingOfferIds = eligibilityResults
            .Select(x => x.Key.FundingOfferId)
            .Distinct()
            .ToList();
        var academicYears = eligibilityResults
            .Select(x => x.AcademicYear)
            .Distinct()
            .ToList();

        var candidates = await context.RolloverCandidates
            .Where(x =>
                sourceQualificationIds.Contains(x.SourceQualificationId) &&
                fundingOfferIds.Contains(x.FundingOfferId) &&
                academicYears.Contains(x.AcademicYear))
            .ToListAsync(cancellationToken);

        var now = clock.UtcNow;
        var created = 0;
        var refreshed = 0;
        var deactivated = 0;
        var reactivated = 0;
        var deactivatedCandidateIds = new List<Guid>();
        var deactivatedCandidateKeys = new Dictionary<Guid, FundingChangeKey>();
        var outcomes = new List<FundingReconciliationOutcome>();

        foreach (var eligibility in eligibilityResults)
        {
            var outcome = ReconcileEligibility(
                eligibility,
                candidates,
                now,
                deactivatedCandidateIds,
                deactivatedCandidateKeys,
                ref created,
                ref refreshed,
                ref deactivated,
                ref reactivated);
            outcomes.Add(outcome);
        }

        var workflowsInvalidated = await InvalidateWorkflowsForDeactivatedCandidatesAsync(
            deactivatedCandidateIds,
            deactivatedCandidateKeys,
            now,
            outcomes,
            cancellationToken);

        return new FundingReconciliationResult(
            created,
            refreshed,
            deactivated,
            reactivated,
            workflowsInvalidated,
            outcomes);
    }

    private FundingReconciliationOutcome ReconcileEligibility(
        RolloverFundingEligibility eligibility,
        List<RolloverCandidates> candidates,
        DateTime now,
        List<Guid> deactivatedCandidateIds,
        Dictionary<Guid, FundingChangeKey> deactivatedCandidateKeys,
        ref int created,
        ref int refreshed,
        ref int deactivated,
        ref int reactivated)
    {
        var matches = candidates
            .Where(x =>
                x.SourceType == eligibility.Key.SourceType &&
                x.SourceQualificationId == eligibility.Key.SourceQualificationId &&
                x.FundingOfferId == eligibility.Key.FundingOfferId &&
                x.AcademicYear == eligibility.AcademicYear)
            .OrderByDescending(x => x.RolloverRound)
            .ToList();

        if (!eligibility.IsEligible)
        {
            return DeactivateCandidates(
                matches,
                eligibility,
                now,
                deactivatedCandidateIds,
                deactivatedCandidateKeys,
                ref deactivated);
        }

        var activeCandidate = matches.FirstOrDefault(x => x.IsActive);
        if (activeCandidate is not null)
        {
            activeCandidate.RefreshFunding(eligibility.FundingEndDate, now);
            refreshed++;
            return new FundingReconciliationOutcome(eligibility.Key, "refreshed", 1);
        }

        var inactiveCandidate = matches.FirstOrDefault();
        if (inactiveCandidate is not null)
        {
            inactiveCandidate.Reactivate(eligibility.FundingEndDate, now);
            reactivated++;
            return new FundingReconciliationOutcome(eligibility.Key, "reactivated", 1);
        }

        var newCandidate = RolloverCandidates.CreateInitialRound(
            eligibility.Key.SourceType,
            eligibility.Key.SourceQualificationId,
            eligibility.Key.FundingOfferId,
            eligibility.AcademicYear,
            now);
        newCandidate.RefreshFunding(eligibility.FundingEndDate, now);
        context.RolloverCandidates.Add(newCandidate);
        candidates.Add(newCandidate);
        created++;
        return new FundingReconciliationOutcome(eligibility.Key, "created", 1);
    }

    private static FundingReconciliationOutcome DeactivateCandidates(
        List<RolloverCandidates> matches,
        RolloverFundingEligibility eligibility,
        DateTime now,
        List<Guid> deactivatedCandidateIds,
        Dictionary<Guid, FundingChangeKey> deactivatedCandidateKeys,
        ref int deactivated)
    {
        var activeMatches = matches.Where(x => x.IsActive).ToList();
        foreach (var candidate in activeMatches)
        {
            candidate.Deactivate(now);
            deactivated++;
            deactivatedCandidateIds.Add(candidate.Id);
            deactivatedCandidateKeys[candidate.Id] = eligibility.Key;
        }

        return new FundingReconciliationOutcome(
            eligibility.Key,
            activeMatches.Count > 0 ? "deactivated" : "ineligible-no-candidate",
            activeMatches.Count);
    }

    private async Task<int> InvalidateWorkflowsForDeactivatedCandidatesAsync(
        List<Guid> deactivatedCandidateIds,
        Dictionary<Guid, FundingChangeKey> deactivatedCandidateKeys,
        DateTime now,
        List<FundingReconciliationOutcome> outcomes,
        CancellationToken cancellationToken)
    {
        if (deactivatedCandidateIds.Count == 0)
        {
            return 0;
        }

        var workflowCandidates = await context.RolloverWorkflowCandidates
            .Where(x =>
                deactivatedCandidateIds.Contains(x.RolloverCandidatesId) &&
                !x.InvalidatedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var workflowCandidate in workflowCandidates)
        {
            workflowCandidate.Invalidate(FundingNoLongerApplicableReason, now);
            outcomes.Add(new FundingReconciliationOutcome(
                deactivatedCandidateKeys[workflowCandidate.RolloverCandidatesId],
                "workflow-invalidated",
                1));
        }

        return workflowCandidates.Count;
    }
}
