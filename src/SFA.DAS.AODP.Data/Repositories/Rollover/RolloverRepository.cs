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

    public async Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesAsync()
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Select(rc => new RolloverCandidate
            {
                Id = rc.Id,
                QualificationVersionId = rc.QualificationVersionId,
                FundingOfferId = rc.FundingOfferId,
                FundingOfferName = rc.FundingOffer != null
                    ? rc.FundingOffer.Name
                    : null,
                QualificationNumber = rc.QualificationVersion != null && rc.QualificationVersion.Qualification != null
                    ? rc.QualificationVersion.Qualification.Qan
                    : null,
                IsActive = rc.IsActive,
                AcademicYear = rc.AcademicYear
            })
            .ToListAsync();
    }

    public async Task<RolloverWorkflowRun> CreateRolloverWorkflowRunAsync(RolloverWorkflowRun workflowRun, CancellationToken cancellationToken = default)
    {
        if (workflowRun == null) throw new ArgumentNullException(nameof(workflowRun));

        _context.RolloverWorkflowRuns.Add(workflowRun);
        await _context.SaveChangesAsync(cancellationToken);
        return workflowRun;
    }

    public async Task AddWorkflowCandidatesAsync(Guid workflowRunId, string academicYear, IReadOnlyCollection<Guid> rolloverCandidateIds, CancellationToken cancellationToken = default)
    {
        if (rolloverCandidateIds == null || rolloverCandidateIds.Count == 0)
            throw new ArgumentException("At least one rollover candidate ID must be provided.", nameof(rolloverCandidateIds));

        var now = DateTime.UtcNow;

        var candidates = await _context.RolloverCandidates
            .Where(rc => rolloverCandidateIds.Contains(rc.Id)
                         && rc.AcademicYear == academicYear
                         && rc.IsActive)
            .ToListAsync(cancellationToken);

        if (candidates.Count != rolloverCandidateIds.Count)
        {
            throw new InvalidOperationException("One or more rollover candidate IDs are invalid or inactive.");
        }

        var workflowCandidates = candidates.Select(rc => RolloverWorkflowCandidate.Create(
            workflowRunId,
            rc.Id,
            rc.QualificationVersionId,
            rc.FundingOfferId,
            rc.AcademicYear,
            rc.PreviousFundingEndDate ?? now,
            rc.NewFundingEndDate,
            now)
        ).ToList();

        _context.RolloverWorkflowCandidates.AddRange(workflowCandidates);

        await _context.SaveChangesAsync(cancellationToken);
    }
}