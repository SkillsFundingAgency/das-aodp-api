using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder;

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

    public async Task<List<View_QuestionRoutingDetail>> GetQuestionRoutingDetailsByQuestionId(Guid questionId)
    {
        return await _context.View_QuestionRoutingDetails.Where(v => v.QuestionId == questionId)
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


    public async Task<bool> IsRouteEditable(Guid id)
    {
        return await _context.Routes.AnyAsync(v => v.Id == id && v.SourceQuestion.Page.Section.FormVersion.Status == FormVersionStatus.Draft.ToString());
    }

    public async Task CopyRoutesForNewFormVersion(Dictionary<Guid, Guid> oldNewQuestionIds,
        Dictionary<Guid, Guid> oldNewPageIds,
        Dictionary<Guid, Guid> oldNewSectionIds,
        Dictionary<Guid, Guid> oldNewOptionIds)
    {
        var sourceQuestionOldIds = oldNewQuestionIds.Keys.ToList();
        var toMigrate = await _context.Routes.AsNoTracking().Where(v => sourceQuestionOldIds.Contains(v.SourceQuestionId)).ToListAsync();
        foreach (var entity in toMigrate)
        {
            entity.SourceQuestionId = oldNewQuestionIds[entity.SourceQuestionId];
            entity.SourceOptionId = oldNewOptionIds[entity.SourceOptionId];

            if (entity.NextPageId != null) entity.NextPageId = oldNewPageIds[entity.NextPageId.Value];
            if (entity.NextSectionId != null) entity.NextSectionId = oldNewSectionIds[entity.NextSectionId.Value];


            entity.Id = Guid.NewGuid();
        }
        await _context.Routes.AddRangeAsync(toMigrate);
        await _context.SaveChangesAsync();
    }
}

