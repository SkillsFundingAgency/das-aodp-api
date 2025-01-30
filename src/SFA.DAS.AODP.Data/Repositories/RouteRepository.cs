using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly IApplicationDbContext _context;

    public RouteRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<View_AvailableQuestionsForRouting>> GetAvailableSectionsAndPagesForRoutingByFormVersionId(Guid formVersionId)
    {
        return await _context.View_AvailableQuestionsForRoutings.Where(v => v.FormVersionId == formVersionId)
            .ToListAsync();
    }

    public async Task<List<View_AvailableQuestionsForRouting>> GetAvailableQuestionsForRoutingByPageId(Guid pageId)
    {
        return await _context.View_AvailableQuestionsForRoutings.Where(v => v.PageId == pageId)
            .ToListAsync();
    }

    public async Task<List<View_QuestionRoutingDetail>> GetQuestionRoutingDetailsByFormVersionId(Guid formVersionId)
    {
        return await _context.View_QuestionRoutingDetails.Where(v => v.FormVersionId == formVersionId)
            .ToListAsync();
    }

    public async Task<List<Route>> GetRoutesByQuestionId(Guid questionId)
    {
        return await _context.Routes.Where(v => v.SourceQuestionId == questionId)
                .ToListAsync();
    }

    public async Task UpsertAsync(List<Route> dbRoutes)
    {
        foreach (var route in dbRoutes)
        {
            if (route.Id == default)
            {
                route.Id = Guid.NewGuid();
                _context.Routes.Add(route);

            }
            else
            {
                _context.Routes.Update(route);
            }
        }
        await _context.SaveChangesAsync();
    }
}

