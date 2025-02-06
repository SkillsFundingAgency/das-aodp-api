﻿using Microsoft.EntityFrameworkCore;
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
        var res = _context.Pages.Where(v => v.SectionId == sectionId).Max(s => (int?)s.Order) ?? 0;
        return res;
    }

    public async Task<List<Page>> GetNextPagesInSectionByOrderAsync(Guid sectionId, int order)
    {
        return await _context.Pages.Where(v => v.SectionId == sectionId && v.Order > order).ToListAsync();
    }

    public async Task<List<Guid>> GetPagesIdInSectionByOrderAsync(Guid sectionId, int startOrderInclusive, int? endOrderExclusive)
    {
        return await _context.Pages.Where(v => v.SectionId == sectionId && v.Order >= startOrderInclusive && (!endOrderExclusive.HasValue || v.Order < endOrderExclusive)).Select(p => p.Id).ToListAsync();
    }

    public async Task<List<Guid>> GetPagesIdInFormBySectionOrderAsync(Guid formVersionId, int startSectionOrderInclusive, int? endSectionOrderInclusive)
    {
        return await _context.Pages.Where(v => v.Section.FormVersionId == formVersionId && v.Section.Order >= startSectionOrderInclusive && (!endSectionOrderInclusive.HasValue || v.Section.Order <= endSectionOrderInclusive)).Select(p => p.Id).ToListAsync();
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

        if (!await _context.Sections.AnyAsync(v => v.Id == page.SectionId && v.FormVersion.Status == FormVersionStatus.Draft.ToString()))
            throw new RecordLockedException();

        page.Id = Guid.NewGuid();
        page.Key = Guid.NewGuid();

        _context.Pages.Add(page);
        await _context.SaveChangesAsync();
        return page;
    }

    /// <summary>
    /// Copies all pages with a given section id, to a new section id. 
    /// Used when creating a new form version from an old one. 
    /// </summary>
    /// <param name="oldSectionId"></param>
    /// <param name="newSectionId"></param>
    /// <returns></returns>
    public async Task<List<Page>> CopyPagesForNewSection(Guid oldSectionId, Guid newSectionId)
    {
        var pagesToMigrate = await GetPagesForSectionAsync(oldSectionId);
        foreach (var p in pagesToMigrate)
        {
            p.SectionId = newSectionId;
        }
        await _context.Pages.AddRangeAsync(pagesToMigrate);
        await _context.SaveChangesAsync();
        return pagesToMigrate;
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

        if (!await _context.Sections.AnyAsync(v => v.Id == page.SectionId && v.FormVersion.Status == FormVersionStatus.Draft.ToString()))
            throw new RecordLockedException();

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

    public async Task UpdatePageOrdering(Guid sectionId, int deletedPageOrder)
    {
        await _context.Pages
            .Where(p => p.SectionId == sectionId && p.Order > deletedPageOrder)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Order, p => p.Order - 1));
    }
}
