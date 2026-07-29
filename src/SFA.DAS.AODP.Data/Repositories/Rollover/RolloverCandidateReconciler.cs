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
                var activeMatches = matches.Where(x => x.IsActive).ToList();
                foreach (var candidate in activeMatches)
                {
                    candidate.Deactivate(now);
                    deactivated++;
                    deactivatedCandidateIds.Add(candidate.Id);
                    deactivatedCandidateKeys[candidate.Id] = eligibility.Key;
                }

                outcomes.Add(new FundingReconciliationOutcome(
                    eligibility.Key,
                    activeMatches.Count > 0 ? "deactivated" : "ineligible-no-candidate",
                    activeMatches.Count));
                continue;
            }

            var activeCandidate = matches.FirstOrDefault(x => x.IsActive);
            if (activeCandidate is not null)
            {
                activeCandidate.RefreshFunding(eligibility.FundingEndDate, now);
                refreshed++;
                outcomes.Add(new FundingReconciliationOutcome(
                    eligibility.Key,
                    "refreshed",
                    1));
                continue;
            }

            var inactiveCandidate = matches.FirstOrDefault();
            if (inactiveCandidate is not null)
            {
                inactiveCandidate.Reactivate(eligibility.FundingEndDate, now);
                reactivated++;
                outcomes.Add(new FundingReconciliationOutcome(
                    eligibility.Key,
                    "reactivated",
                    1));
                continue;
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
            outcomes.Add(new FundingReconciliationOutcome(
                eligibility.Key,
                "created",
                1));
        }

        var workflowsInvalidated = 0;
        if (deactivatedCandidateIds.Count > 0)
        {
            var workflowCandidates = await context.RolloverWorkflowCandidates
                .Where(x =>
                    deactivatedCandidateIds.Contains(x.RolloverCandidatesId) &&
                    !x.InvalidatedAt.HasValue)
                .ToListAsync(cancellationToken);

            foreach (var workflowCandidate in workflowCandidates)
            {
                workflowCandidate.Invalidate(FundingNoLongerApplicableReason, now);
                workflowsInvalidated++;
                outcomes.Add(new FundingReconciliationOutcome(
                    deactivatedCandidateKeys[workflowCandidate.RolloverCandidatesId],
                    "workflow-invalidated",
                    1));
            }
        }

        return new FundingReconciliationResult(
            created,
            refreshed,
            deactivated,
            reactivated,
            workflowsInvalidated,
            outcomes);
    }
}
