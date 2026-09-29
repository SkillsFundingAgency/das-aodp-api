using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Funding;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public sealed class FundingDomainEventDispatcher(
    ISystemClockProvider clock,
    ILogger<FundingDomainEventDispatcher> logger) : IFundingDomainEventDispatcher
{
    private const string SourceMovedReason =
        "The Ofqual funding moved to a newer qualification version.";

    public async Task DispatchAsync(
        ApplicationDbContext context,
        IReadOnlyCollection<FundingDomainEvent> events,
        CancellationToken cancellationToken)
    {
        var changedEvents = events.OfType<FundingChangedDomainEvent>().Distinct().ToList();
        var eligibilityEvents = events
            .OfType<QualificationFundingEligibilityChangedDomainEvent>()
            .Distinct()
            .ToList();
        var keys = new List<FundingChangeKey>();

        foreach (var changedEvent in changedEvents)
        {
            var moveHandled = changedEvent.PreviousSourceQualificationId.HasValue &&
                              await MoveActiveOfqualCandidatesAsync(
                                  context,
                                  changedEvent,
                                  cancellationToken);
            if (!moveHandled)
            {
                keys.Add(new FundingChangeKey(
                    changedEvent.SourceType,
                    changedEvent.SourceQualificationId,
                    changedEvent.FundingOfferId));
            }
        }

        keys.AddRange(await ExpandEligibilityChangesAsync(
            context,
            eligibilityEvents,
            cancellationToken));

        if (keys.Count == 0)
        {
            return;
        }

        var eligibilityRepository = new RolloverFundingEligibilityRepository(
            context,
            new AcademicYearProvider(clock));
        var reconciler = new RolloverCandidateReconciler(
            context,
            eligibilityRepository,
            clock);
        var result = await reconciler.ReconcileAsync(
            keys.Distinct().ToList(),
            cancellationToken);

        logger.LogInformation(
            "Funding events reconciled. Keys: {KeyCount}, created: {Created}, refreshed: {Refreshed}, deactivated: {Deactivated}, reactivated: {Reactivated}, workflows invalidated: {Invalidated}.",
            keys.Count,
            result.Created,
            result.Refreshed,
            result.Deactivated,
            result.Reactivated,
            result.WorkflowsInvalidated);
    }

    private async Task<bool> MoveActiveOfqualCandidatesAsync(
        ApplicationDbContext context,
        FundingChangedDomainEvent changedEvent,
        CancellationToken cancellationToken)
    {
        if (changedEvent.SourceType != RolloverSourceTypes.Ofqual)
        {
            throw new InvalidOperationException(
                "Only Ofqual funding can move between qualification versions.");
        }

        var previousSourceQualificationId = changedEvent.PreviousSourceQualificationId!.Value;
        var oldCandidates = await context.RolloverCandidates
            .Where(candidate =>
                candidate.SourceType == RolloverSourceTypes.Ofqual &&
                candidate.SourceQualificationId == previousSourceQualificationId &&
                candidate.FundingOfferId == changedEvent.FundingOfferId &&
                candidate.IsActive)
            .ToListAsync(cancellationToken);

        if (oldCandidates.Count == 0)
        {
            return false;
        }

        var academicYears = oldCandidates.Select(candidate => candidate.AcademicYear).Distinct().ToList();
        var targetCandidates = await context.RolloverCandidates
            .Where(candidate =>
                candidate.SourceType == RolloverSourceTypes.Ofqual &&
                candidate.SourceQualificationId == changedEvent.SourceQualificationId &&
                candidate.FundingOfferId == changedEvent.FundingOfferId &&
                academicYears.Contains(candidate.AcademicYear) &&
                candidate.IsActive)
            .ToListAsync(cancellationToken);

        var now = clock.UtcNow;
        foreach (var oldCandidate in oldCandidates)
        {
            var targetExists = targetCandidates.Any(target =>
                target.AcademicYear == oldCandidate.AcademicYear);
            if (targetExists)
            {
                oldCandidate.Deactivate(now);
            }
            else
            {
                oldCandidate.MoveSourceQualification(
                    changedEvent.SourceQualificationId,
                    now);
            }
        }

        var oldCandidateIds = oldCandidates.Select(candidate => candidate.Id).ToList();
        var workflows = await context.RolloverWorkflowCandidates
            .Where(candidate =>
                oldCandidateIds.Contains(candidate.RolloverCandidatesId) &&
                !candidate.InvalidatedAt.HasValue)
            .ToListAsync(cancellationToken);
        foreach (var workflow in workflows)
        {
            workflow.Invalidate(SourceMovedReason, now);
        }

        logger.LogInformation(
            "Moved {CandidateCount} active rollover candidates from qualification version {PreviousVersionId} to {NewVersionId} for funding offer {FundingOfferId}; invalidated {WorkflowCount} workflow snapshots.",
            oldCandidates.Count,
            previousSourceQualificationId,
            changedEvent.SourceQualificationId,
            changedEvent.FundingOfferId,
            workflows.Count);

        return true;
    }

    private static async Task<IReadOnlyCollection<FundingChangeKey>> ExpandEligibilityChangesAsync(
        ApplicationDbContext context,
        IReadOnlyCollection<QualificationFundingEligibilityChangedDomainEvent> events,
        CancellationToken cancellationToken)
    {
        var ofqualVersionIds = events
            .Where(domainEvent => domainEvent.SourceType == RolloverSourceTypes.Ofqual)
            .Select(domainEvent => domainEvent.SourceQualificationId)
            .Distinct()
            .ToList();
        var qaaQualificationIds = events
            .Where(domainEvent => domainEvent.SourceType == RolloverSourceTypes.Qaa)
            .Select(domainEvent => domainEvent.SourceQualificationId)
            .Distinct()
            .ToList();

        var keys = await context.QualificationFundings
            .AsNoTracking()
            .Where(funding => ofqualVersionIds.Contains(funding.QualificationVersionId))
            .Select(funding => new FundingChangeKey(
                RolloverSourceTypes.Ofqual,
                funding.QualificationVersionId,
                funding.FundingOfferId))
            .ToListAsync(cancellationToken);

        keys.AddRange(await context.QaaQualificationFundings
            .AsNoTracking()
            .Where(funding => qaaQualificationIds.Contains(funding.QaaQualificationId))
            .Select(funding => new FundingChangeKey(
                RolloverSourceTypes.Qaa,
                funding.QaaQualificationId,
                funding.FundingOfferId))
            .ToListAsync(cancellationToken));

        return keys;
    }
}
