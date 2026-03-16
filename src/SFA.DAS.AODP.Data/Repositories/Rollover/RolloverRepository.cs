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
        return  await _context.RolloverCandidates
        .AsNoTracking()
        .Select(rc => new RolloverCandidate
        {
            Id = rc.Id,
            QualificationVersionId = rc.QualificationVersionId,
            FundingOfferId = rc.FundingOfferId,
            Qan = rc.QualificationVersion.Qualification.Qan,
            Title = rc.QualificationVersion.Qualification.QualificationName ?? string.Empty,
            AwardingOrganisation = rc.QualificationVersion.Organisation.NameOfqual ?? string.Empty,
            FundingOffer = rc.FundingOffer.Name,
            FundingApprovalEndDate = rc.NewFundingEndDate,
            IsActive = rc.IsActive
        })
        .ToListAsync();
    }
}