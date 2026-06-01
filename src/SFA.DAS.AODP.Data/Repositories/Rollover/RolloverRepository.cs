using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class RolloverRepository : IRolloverRepository
{
    private readonly IApplicationDbContext _context;

    public RolloverRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken)
    {
        var dbSet = _context.RolloverWorkflowCandidates;

        var totalRecords = await dbSet.AsNoTracking().CountAsync(cancellationToken);

        return totalRecords;
    }

    public async Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        var dbSet = _context.RolloverWorkflowCandidates;

        var query = dbSet.AsNoTracking();

        var data = await query
                        .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<IEnumerable<RolloverWorkflowCandidatesP1Checks>> GetRolloverWorkflowCandidatesP1ChecksAsync(CancellationToken cancellationToken)
    {
        var dbSet = _context.RolloverWorkflowCandidatesP1Checks;
        var query = await dbSet.AsNoTracking().ToListAsync(cancellationToken);

        return query;
    }

    public async Task UpdateRolloverWorkflowCandidatesAsync(IEnumerable<RolloverWorkflowCandidate> candidates, CancellationToken cancellationToken)
    {
        var list = candidates as IList<RolloverWorkflowCandidate> ?? candidates.ToList();
        if (!list.Any())
            return;

        _context.RolloverWorkflowCandidates.UpdateRange(list);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(rc => new RolloverCandidate
            {
                Id = rc.Id,
                QualificationVersionId = rc.QualificationVersionId,
                FundingOfferId = rc.FundingOfferId,
                FundingOfferName = rc.FundingOffer != null ? rc.FundingOffer.Name : null,
                QualificationNumber = rc.QualificationVersion != null && rc.QualificationVersion.Qualification != null ?
                    rc.QualificationVersion.Qualification.Qan : null,
                AcademicYear = rc.AcademicYear
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(rc =>
                rolloverCandidateIds.Contains(rc.Id) && rc.IsActive)
            .Select(rc => new RolloverCandidate
            {
                Id = rc.Id,
                QualificationVersionId = rc.QualificationVersionId,
                FundingOfferId = rc.FundingOfferId,
                RolloverRound = rc.RolloverRound,
                AcademicYear = rc.AcademicYear,
                PreviousFundingEndDate = rc.PreviousFundingEndDate,
                NewFundingEndDate = rc.NewFundingEndDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> CreateRolloverWorkflowRunAsync(RolloverWorkflowRun workflowRun, CancellationToken cancellationToken = default)
    {
        _context.RolloverWorkflowRuns.Add(workflowRun);
        await _context.SaveChangesAsync(cancellationToken);
        return workflowRun.Id;
    }

    public async Task CreateRolloverWorkflowCandidatesAsync(
        IEnumerable<RolloverWorkflowCandidate> workflowCandidates,
        CancellationToken cancellationToken)
    {
        var incomingRolloverCandidates = workflowCandidates.ToList();

        var incomingCandidateIds = incomingRolloverCandidates
            .Select(x => x.RolloverCandidatesId)
            .ToList();

        var existingWorkflowCandidates = await _context.RolloverWorkflowCandidates
            .Where(x => incomingCandidateIds.Contains(x.RolloverCandidatesId))
            .ToListAsync(cancellationToken);

        _context.RolloverWorkflowCandidates.RemoveRange(existingWorkflowCandidates);

        _context.RolloverWorkflowCandidates.AddRange(incomingRolloverCandidates);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRolloverWorkflowRunFundingOffersAsync(IEnumerable<RolloverWorkflowRunFundingOffer> workflowFundingOffers, CancellationToken cancellationToken)
    {
        _context.RolloverWorkflowRunFundingOffers.AddRange(workflowFundingOffers);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RolloverWorkflowCandidatesExportRow>> GetRolloverWorkflowCandidatesByRunId(
        Guid workflowRunId,
        CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc => rwc.RolloverWorkflowRunId == workflowRunId
                       && rwc.IncludedInP1Export)
            .Select(rwc => new RolloverWorkflowCandidatesExportRow
            {
                QAN = rwc.RolloverCandidates.QualificationVersion.Qualification.Qan,
                QualificationTitle = rwc.RolloverCandidates.QualificationVersion.Qualification.QualificationName ?? string.Empty,
                AwardingOrganisation = rwc.RolloverCandidates.QualificationVersion.Organisation.NameOfqual ?? string.Empty,
                QualificationLevel = rwc.RolloverCandidates.QualificationVersion.Level,
                QualificationType = rwc.RolloverCandidates.QualificationVersion.Type,
                SSA = rwc.RolloverCandidates.QualificationVersion.Ssa,
                OperationalEndDate = rwc.RolloverCandidates.QualificationVersion.OperationalEndDate,

                OfferedInEngland = rwc.RolloverCandidates.QualificationVersion.OfferedInEngland,
                FundedInEngland = rwc.RolloverCandidates.QualificationVersion.IntentionToSeekFundingInEngland ?? false,

                GLH = rwc.RolloverCandidates.QualificationVersion.Glh,
                TQT = rwc.RolloverCandidates.QualificationVersion.Tqt,

                Pre16 = rwc.RolloverCandidates.QualificationVersion.PreSixteen ?? false,
                Age16To18 = rwc.RolloverCandidates.QualificationVersion.SixteenToEighteen ?? false,
                Age18Plus = rwc.RolloverCandidates.QualificationVersion.EighteenPlus ?? false,
                Age19Plus = rwc.RolloverCandidates.QualificationVersion.NineteenPlus ?? false,

                FundingStreamName = rwc.RolloverCandidates.FundingOffer.Name,
                FundingApprovalStartDate =
                    _context.QualificationFundings
                        .Where(qf =>
                            qf.QualificationVersionId == rwc.RolloverCandidates.QualificationVersionId &&
                            qf.FundingOfferId == rwc.RolloverCandidates.FundingOfferId)
                        .Select(qf => qf.StartDate)
                        .FirstOrDefault(),

                ProposedOutcome = rwc.PassP1 ? "To Extend" : "To Exclude",
                RolloverStatus = rwc.RolloverCandidates.RolloverStatus.ToString(),
                ExclusionReason = rwc.RolloverCandidates.ExclusionReason,

                CurrentFundingApprovalEndDate = rwc.CurrentFundingEndDate,
                ProposedFundingApprovalEndDate = rwc.ProposedFundingEndDate,

                Comments = string.Empty,
            })
            .OrderBy(x => x.QAN)
            .ToListAsync(cancellationToken);
    }


}