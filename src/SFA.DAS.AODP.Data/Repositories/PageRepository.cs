using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Repositories;

public class PageRepository : IPageRepository
{
    private readonly IApplicationDbContext _context;
    public PageRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Page>> GetPagesForSectionAsync(Guid sectionId)
    {
        return await _context.Pages.Where(v => v.SectionId == sectionId && v.Archived == false).ToListAsync();
    }

    public async Task<Page?> GetPageByIdAsync(Guid pageId)
    {
        return await _context.Pages.FirstOrDefaultAsync(v => v.Id == pageId && v.Archived == false);
    }

    public async Task<Page> Create(Page page)
    {
        _context.Pages.Add(page);
        await _context.SaveChangesAsync();
        return page;
    }

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

    public async Task<Page?> Update(Page page)
    {
        var pageToUpdate = await _context.Pages.FirstOrDefaultAsync(v => v.Id == page.Id && v.Archived == false);
        if (pageToUpdate is null)
            return null;
        pageToUpdate = page;
        await _context.SaveChangesAsync();
        return pageToUpdate;
    }

    public async Task<Page?> Archive(Guid pageId)
    {
        var pageToUpdate = await _context.Pages.FirstOrDefaultAsync(v => v.Id == pageId);
        if (pageToUpdate is null)
            return null;
        pageToUpdate.Archived = true;
        await _context.SaveChangesAsync();
        return pageToUpdate;
    }
}
