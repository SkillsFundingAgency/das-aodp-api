using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Infrastructure.Context;
using SFA.DAS.AODP.Models.Form;

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

    /// <summary>
    /// Gets all sections for a given form version Id. 
    /// Does not check if the form version Id is valid, will just return an empty list if so. 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <returns></returns>
    public async Task<List<Section>> GetSectionsForFormAsync(Guid formVersionId)
    {
        return await _context.Sections.Where(v => v.FormVersionId == formVersionId).ToListAsync();
    }

    /// <summary>
    /// Gets a section with a given Id. 
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<Section> GetSectionByIdAsync(Guid sectionId)
    {
        var res = await _context.Sections.Include(s => s.Pages).FirstOrDefaultAsync(v => v.Id == sectionId);
        if (res is null)
            throw new RecordNotFoundException(sectionId);

        return res;
    }

    /// <summary>
    /// Gets a section with a given Id. 
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<int> GetMaxOrderByFormVersionIdAsync(Guid formVersionId)
    {
        var res = _context.Sections.Where(v => v.FormVersionId == formVersionId).Max(s => (int?)s.Order) ?? 0;
        return res;
    }

    /// <summary>
    /// Creates a new section, throws if the form version Id is not found. 
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    /// <exception cref="NoForeignKeyException"></exception>
    public async Task<Section> Create(Section section)
    {
        if (!await _context.FormVersions.AnyAsync(v => v.Id == section.FormVersionId))
            throw new NoForeignKeyException(section.FormVersionId);

        section.Id = Guid.NewGuid();
        section.Key = Guid.NewGuid();

        _context.Sections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }

    /// <summary>
    /// Copies all sections associated with one form version id to another. 
    /// Used when creating a new form version from an old one. 
    /// </summary>
    /// <param name="oldFormVersionId"></param>
    /// <param name="newFormVersionId"></param>
    /// <returns></returns>
    public async Task<List<Section>> CopySectionsForNewForm(Guid oldFormVersionId, Guid newFormVersionId)
    {
        var sectionsToMigrate = await GetSectionsForFormAsync(oldFormVersionId);
        foreach (var s in sectionsToMigrate)
        {
            var oldSectionId = s.Id;
            s.Id = Guid.NewGuid();
            s.FormVersionId = newFormVersionId;
            await _pageRepository.CopyPagesForNewSection(oldSectionId, s.Id);
        }
        await _context.Sections.AddRangeAsync(sectionsToMigrate);
        await _context.SaveChangesAsync();
        return sectionsToMigrate;
    }

    /// <summary>
    /// Updates a section's data, throws if no section for the given Id can be found, 
    /// or if the linked form version is not in draft. 
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    public async Task<Section> Update(Section section)
    {
        if (_context.FormVersions.Any(v => v.Id == section.FormVersionId && v.Status != FormVersionStatus.Draft.ToString()))
            throw new RecordLockedException();
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
        return section;
    }

    /// <summary>
    /// Deletes a section with a given Id, throws if no record can be found for a given Id. 
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<Section> DeleteSection(Guid sectionId)
    {
        var sectionToUpdate = await _context.Sections.FirstOrDefaultAsync(v => v.Id == sectionId);
        if (sectionToUpdate is null)
            throw new RecordNotFoundException(sectionId);
        _context.Sections.Remove(sectionToUpdate);
        await _context.SaveChangesAsync();
        return sectionToUpdate;
    }
}
