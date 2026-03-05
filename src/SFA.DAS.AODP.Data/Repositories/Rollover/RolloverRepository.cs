using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Models.Rollover;
using RolloverModel = SFA.DAS.AODP.Models.Rollover.RolloverWorkflowCandidate;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class RolloverRepository : IRolloverRepository
{
    private readonly IApplicationDbContext _context;

    public RolloverRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RolloverWorkflowCandidatesResult> GetAllRolloverWorkflowCandidatesAsync(int? skip = 0, int? take = 0)
    {
        var dbSet = _context.RolloverWorkflowCandidates;
        if (dbSet == null)
        {
            return new RolloverWorkflowCandidatesResult
            {
                Data = new List<RolloverModel>(),
                Skip = skip,
                Take = take,
                TotalRecords = 0
            };
        }

        var query = dbSet.AsNoTracking();
        var totalRecords = await query.CountAsync();
        if (totalRecords == 0)
        {
            return new RolloverWorkflowCandidatesResult
            {
                Data = new List<RolloverModel>(),
                Skip = skip,
                Take = take,
                TotalRecords = 0
            };
        }

        var skipChecked = skip ?? 0;
        var takeChecked = take ?? 50;

        try
        {
            var data = await query
                            .OrderByDescending(r => r.CreatedAt)
                            .Skip(skipChecked)
                            .Take(takeChecked)
                            .Select(e => new RolloverModel
                            {
                                Id = e.Id,
                                RolloverWorkflowRunId = e.RolloverWorkflowRunId,
                                QualificationVersionId = e.QualificationVersionId,
                                FundingOfferId = e.FundingOfferId,
                                AcademicYear = e.AcademicYear,
                                //RolloverCandidateId = e.RolloverCandidateId,
                                PassP1 = e.PassP1,
                                P1FailureReason = e.P1FailureReason,
                                IncludedInP1Export = e.IncludedInP1Export,
                                IncludedInFinalUpload = e.IncludedInFinalUpload,
                                CurrentFundingEndDate = e.CurrentFundingEndDate,
                                ProposedFundingEndDate = e.ProposedFundingEndDate,
                                CreatedAt = e.CreatedAt,
                                UpdatedAt = e.UpdatedAt
                            })
                            .ToListAsync();

            return new RolloverWorkflowCandidatesResult
            {
                Data = data,
                Skip = skip,
                Take = take,
                TotalRecords = totalRecords
            };
        }
        catch (Exception)
        {

            throw;
        }
        
    }
}