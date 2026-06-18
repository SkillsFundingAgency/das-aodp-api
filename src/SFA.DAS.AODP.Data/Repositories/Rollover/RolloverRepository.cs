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
        return await _context.RolloverWorkflowCandidates.ToListAsync(cancellationToken);
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

    public async Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(rc => new RolloverCandidateDto
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

    public async Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(rc =>
                rolloverCandidateIds.Contains(rc.Id) && rc.IsActive)
            .Select(rc => new RolloverCandidateDto
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

    public async Task<IReadOnlyList<RolloverCandidateForExport>> GetRolloverWorkflowCandidatesByRunId(
        Guid workflowRunId,
        CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc => rwc.RolloverWorkflowRunId == workflowRunId
                       && rwc.IncludedInP1Export)
            .Select(rwc => new RolloverCandidateForExport
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
                RolloverStatus = rwc.RolloverCandidates.RolloverStatus,
                ExclusionReason = rwc.PassP1 ? rwc.RolloverCandidates.ExclusionReason : rwc.P1FailureReason,

                CurrentFundingApprovalEndDate = rwc.CurrentFundingEndDate,
                ProposedFundingApprovalEndDate = rwc.ProposedFundingEndDate,

                Comments = string.Empty,
            })
            .OrderBy(x => x.QAN)
            .ToListAsync(cancellationToken);
    }
    public async Task<RolloverWorkflowRun> GeRolloverWorkflowRunByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowRuns
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<FundingExtensionCandidateValidationContext> GetFundingExtensionValidationContextAsync(
        HashSet<CandidateKey> incomingCandidates,
        CancellationToken cancellationToken)
    {
        var flattened = incomingCandidates
            .Select(k => $"{k.Qan}|{k.FundingStream}")
            .ToList();

        var latestRunId = await GetLatestWorkflowRunIdAsync(cancellationToken);

        if (latestRunId == Guid.Empty)
            throw new InvalidOperationException("No workflow runs exist");

        var matchingCandidatesInDB = await _context.RolloverCandidates
            .AsNoTracking()
            .Where(rc =>
                flattened.Contains(
                    rc.QualificationVersion.Qualification.Qan + "|" +
                    rc.FundingOffer.Name))
            .Select(rc => new CandidateKey(
                rc.QualificationVersion.Qualification.Qan,
                rc.FundingOffer.Name))
            .ToHashSetAsync(cancellationToken);

        var matchingWorkflowCandidatesInDB = await _context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc => rwc.RolloverWorkflowRunId == latestRunId)
            .Where(rwc =>
                flattened.Contains(
                    rwc.QualificationVersion.Qualification.Qan + "|" +
                    rwc.FundingOffer.Name))
            .Select(rwc => new CandidateKey(
                rwc.QualificationVersion.Qualification.Qan,
                rwc.FundingOffer.Name))
            .ToHashSetAsync(cancellationToken);

        return new FundingExtensionCandidateValidationContext(
            incomingCandidates,
            matchingCandidatesInDB,
            matchingWorkflowCandidatesInDB
        );
    }

    public async Task<List<RolloverCandidateStatusItem>> GetRolloverCandidatesStatusAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .Select(x => new RolloverCandidateStatusItem
            {
                Qan = x.QualificationVersion.Qualification.Qan,
                FundingStreamName = x.FundingOffer.Name,
                RolloverStatus = x.RolloverStatus
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RolloverCandidates>> LoadRolloverCandidateGraphAsync(
        List<CandidateKey> keys,
        CancellationToken cancellationToken)
    {
        var keySet = keys
            .Select(x => x.Qan + "|" + x.FundingStream)
            .ToHashSet();

        return await _context.RolloverCandidates
            .Include(x => x.QualificationVersion)
                .ThenInclude(v => v.Qualification)
            .Include(x => x.FundingOffer)
            .Where(x =>
                keySet.Contains(
                    x.QualificationVersion.Qualification.Qan + "|" +
                    x.FundingOffer.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAllWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        var items = await _context.RolloverWorkflowCandidates
            .ToListAsync(cancellationToken);

        _context.RolloverWorkflowCandidates.RemoveRange(items);
    }

    public async Task<Guid?> GetLatestWorkflowRunIdAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowRuns
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}