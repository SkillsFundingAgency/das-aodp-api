using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Repositories;

public class SectionRepository : ISectionRepository
{
    private readonly IApplicationDbContext _context;
    private readonly IPageRepository _pageRepository;
    public SectionRepository(IApplicationDbContext context, IPageRepository pageRepository)
    {
        _context = context;
        _pageRepository = pageRepository;
    }

    public async Task<List<Section>> GetSectionsForFormAsync(Guid formId)
    {
        return await _context.Sections.Where(v => v.FormId == formId && v.Archived == false).ToListAsync();
    }

    public async Task<Section?> GetSectionByIdAsync(Guid sectionId)
    {
        return await _context.Sections.FirstOrDefaultAsync(v => v.Id == sectionId && v.Archived == false);
    }

    public async Task<Section> Create(Section section)
    {
        _context.Sections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }

    public async Task<List<Section>> CopySectionsForNewForm(Guid oldFormId, Guid newFormId)
    {
        var sectionsToMigrate = await GetSectionsForFormAsync(oldFormId);
        foreach (var s in sectionsToMigrate)
        {
            var oldSectionId = s.Id;
            s.Id = Guid.NewGuid();
            s.FormId = newFormId;
            await _pageRepository.CopyPagesForNewSection(oldSectionId, s.Id);
        }
        await _context.Sections.AddRangeAsync(sectionsToMigrate);
        await _context.SaveChangesAsync();
        return sectionsToMigrate;
    }

    public async Task<Section?> Update(Section section)
    {
        var sectionToUpdate = await _context.Sections.FirstOrDefaultAsync(v => v.Id == section.Id && v.Archived == false);
        if (sectionToUpdate is null)
            return null;
        sectionToUpdate = section;
        await _context.SaveChangesAsync();
        return section;
    }

    public async Task<Section?> ArchiveSection(Guid sectionId)
    {
        var sectionToUpdate = await _context.Sections.FirstOrDefaultAsync(v => v.Id == sectionId);
        if (sectionToUpdate is null)
            return null;
        sectionToUpdate.Archived = true;
        await _context.SaveChangesAsync();
        return sectionToUpdate;
    }
}
