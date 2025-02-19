using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder;
public class PageRepository : IPageRepository
{
    private readonly IApplicationDbContext _context;
    public PageRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all pages for a given section. 
    /// Does not check if the section Id is valid, retuns an empty list if so.  
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    public async Task<List<Page>> GetPagesForSectionAsync(Guid sectionId)
    {
        return await _context.Pages.Where(v => v.SectionId == sectionId).ToListAsync();
    }

    /// <summary>
    /// Gets max order for pages for given section id.
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public int GetMaxOrderBySectionId(Guid sectionId)
    {
        return _context.Pages.Where(v => v.SectionId == sectionId).Max(s => (int?)s.Order) ?? 0;
    }

    public async Task<List<Page>> GetNextPagesInSectionByOrderAsync(Guid sectionId, int order)
    {
        return await _context.Pages.Where(v => v.SectionId == sectionId && v.Order > order).ToListAsync();
    }

    public async Task<List<Guid>> GetPagesIdInSectionByOrderAsync(Guid sectionId, int startOrderInclusive, int? endOrderExclusive)
    {
        return await _context.Pages.Where(v => v.SectionId == sectionId && v.Order >= startOrderInclusive && (!endOrderExclusive.HasValue || v.Order < endOrderExclusive)).Select(p => p.Id).ToListAsync();
    }

    public async Task<List<Guid>> GetPagesIdInFormBySectionOrderAsync(Guid formVersionId, int startSectionOrderInclusive, int? endSectionOrderExclusive)
    {
        return await _context.Pages.Where(v => v.Section.FormVersionId == formVersionId && v.Section.Order >= startSectionOrderInclusive && (!endSectionOrderExclusive.HasValue || v.Section.Order < endSectionOrderExclusive)).Select(p => p.Id).ToListAsync();
    }

    /// <summary>
    /// Gets a page with a given Id, throws if no page is found with the given Id. 
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<Page> GetPageByIdAsync(Guid pageId)
    {
        var res = await _context.Pages.Include(p => p.Questions).Include(p => p.Section).FirstOrDefaultAsync(v => v.Id == pageId);
        if (res is null)
            throw new RecordNotFoundException(pageId);

        return res;
    }

    /// <summary>
    /// Creates a page on a section, throws if no linked section is found, 
    /// or if the linked form version isn't in draft status. 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    /// <exception cref="NoForeignKeyException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    public async Task<Page> Create(Page page)
    {
        if (!await _context.Sections.AnyAsync(v => v.Id == page.SectionId))
            throw new NoForeignKeyException(page.SectionId);

        page.Id = Guid.NewGuid();
        page.Key = Guid.NewGuid();

        _context.Pages.Add(page);
        await _context.SaveChangesAsync();
        return page;
    }


    public async Task<Dictionary<Guid, Guid>> CopyPagesForNewFormVersion(Dictionary<Guid, Guid> oldNewSectionIds)
    {
        var oldNewIds = new Dictionary<Guid, Guid>();

        var oldIds = oldNewSectionIds.Keys.ToList();
        var pagesToMigrate = await _context.Pages.AsNoTracking().Where(v => oldIds.Contains(v.SectionId)).ToListAsync();

        foreach (var page in pagesToMigrate)
        {
            var oldPageId = page.Id;
            page.SectionId = oldNewSectionIds[page.SectionId];
            page.Id = Guid.NewGuid();

            oldNewIds.Add(oldPageId, page.Id);
        }
        await _context.Pages.AddRangeAsync(pagesToMigrate);
        await _context.SaveChangesAsync();

        return oldNewIds;
    }

    /// <summary>
    /// Updates a page, throws is no page with a given Id is found, 
    /// or the linked form version isn't in draft. 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    public async Task<Page> Update(Page page)
    {
        var pageToUpdate = await _context.Pages.FirstOrDefaultAsync(v => v.Id == page.Id);
        if (pageToUpdate is null)
            throw new RecordNotFoundException(page.Id);

        pageToUpdate = page;
        await _context.SaveChangesAsync();
        return pageToUpdate;
    }

    /// <summary>
    /// Archives a page, throws is no page with a given Id is found, 
    /// or the linked form version isn't in draft. 
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    public async Task<Page> Archive(Guid pageId)
    {
        var pageToUpdate = await _context.Pages
            .Include(p => p.Questions)
                .ThenInclude(q => q.QuestionValidation)
            .Include(p => p.Questions)
                .ThenInclude(q => q.QuestionOptions)
            .FirstOrDefaultAsync(v => v.Id == pageId);

        if (pageToUpdate is null)
            throw new RecordNotFoundException(pageId);

        if (await _context.Sections.AnyAsync(v => v.Id == pageToUpdate.SectionId && v.FormVersion.Status != FormVersionStatus.Draft.ToString()))
            throw new RecordLockedException();

        _context.Pages.Remove(pageToUpdate);
        await _context.SaveChangesAsync();

        await UpdatePageOrdering(pageToUpdate.SectionId, pageToUpdate.Order);

        return pageToUpdate;
    }

    public async Task<Page> GetPageForApplicationAsync(int pageOrder, Guid sectionId)
    {
        return await _context.Pages.Where(p => p.Order == pageOrder && p.SectionId == sectionId)

            .Include(p => p.Questions)
            .ThenInclude(q => q.QuestionValidation)

            .Include(p => p.Questions)
            .ThenInclude(q => q.QuestionOptions)

            .Include(p => p.Section)
            .ThenInclude(s => s.View_SectionPageCount)

            .Include(q => q.Questions)
            .ThenInclude(q => q.Routes)
            .ThenInclude(r => r.NextPage)

            .Include(q => q.Questions)
            .ThenInclude(q => q.Routes)
            .ThenInclude(r => r.NextSection)

            .FirstOrDefaultAsync() ?? throw new RecordNotFoundException(sectionId);
    }

    private async Task UpdatePageOrdering(Guid sectionId, int deletedPageOrder)
    {
        await _context.Pages
            .Where(p => p.SectionId == sectionId && p.Order > deletedPageOrder)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Order, p => p.Order - 1));
    }

    /// <summary>
    /// Finds a question with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MovePageOrderUp(Guid id)
    {
        var modelToUpdate = await _context.Pages.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextHigherModel = await _context.Pages
            .OrderByDescending(v => v.Order)
            .Where(v => v.Order < modelToUpdate.Order && v.SectionId == modelToUpdate.SectionId)
            .FirstOrDefaultAsync();
        if (nextHigherModel is null)
            return true;
        var nextHighest = nextHigherModel.Order;
        nextHigherModel.Order = modelToUpdate.Order;
        modelToUpdate.Order = nextHighest;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Finds a question with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MovePageOrderDown(Guid id)
    {
        var modelToUpdate = await _context.Pages.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextLowerModel = await _context.Pages
            .OrderBy(v => v.Order)
            .Where(v => v.Order > modelToUpdate.Order && v.SectionId == modelToUpdate.SectionId)
            .FirstOrDefaultAsync();
        if (nextLowerModel is null)
            return true;
        var nextLowest = nextLowerModel.Order;
        nextLowerModel.Order = modelToUpdate.Order;
        modelToUpdate.Order = nextLowest;

        _context.Pages.UpdateRange([nextLowerModel, modelToUpdate]);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsPageEditable(Guid id)
    {
        return await _context.Pages.AnyAsync(v => v.Id == id && v.Section.FormVersion.Status == FormVersionStatus.Draft.ToString());
    }
}
