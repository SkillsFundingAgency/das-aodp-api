using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Infrastructure.Context;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Repositories;

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

    /// <summary>
    /// Gets a page with a given Id, throws if no page is found with the given Id. 
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<Page> GetPageByIdAsync(Guid pageId)
    {
        var res = await _context.Pages.FirstOrDefaultAsync(v => v.Id == pageId);
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
        var pageToUpdate = await _context.Pages.FirstOrDefaultAsync(v => v.Id == pageId);
        if (pageToUpdate is null)
            throw new RecordNotFoundException(pageId);

        if (!await _context.Sections.AnyAsync(v => v.Id == pageToUpdate.SectionId && v.FormVersion.Status != FormVersionStatus.Draft.ToString()))
            throw new RecordLockedException();

        _context.Pages.Remove(pageToUpdate);
        await _context.SaveChangesAsync();
        return pageToUpdate;
    }
}
