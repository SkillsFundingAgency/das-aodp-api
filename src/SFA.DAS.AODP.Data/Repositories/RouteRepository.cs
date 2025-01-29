using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
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
}

