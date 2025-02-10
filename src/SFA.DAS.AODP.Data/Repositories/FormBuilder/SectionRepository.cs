using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder;
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


    public int GetMaxOrderByFormVersionId(Guid formVersionId)
    {
        return _context.Sections.Where(v => v.FormVersionId == formVersionId).Max(s => (int?)s.Order) ?? 0;

    }

    public async Task<List<Section>> GetNextSectionsByOrderAsync(Guid formVersionId, int order)
    {
        return await _context.Sections.Where(v => v.FormVersionId == formVersionId && v.Order > order).ToListAsync();
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
    public async Task<Dictionary<Guid, Guid>> CopySectionsForNewFormVersion(Guid oldFormVersionId, Guid newFormVersionId)
    {
        var oldNewIds = new Dictionary<Guid, Guid>();
        var sectionsToMigrate = await _context.Sections.AsNoTracking().Where(v => v.FormVersionId == oldFormVersionId).ToListAsync();
        foreach (var section in sectionsToMigrate)
        {
            var oldSectionId = section.Id;

            section.Id = Guid.NewGuid();
            section.FormVersionId = newFormVersionId;

            oldNewIds.Add(oldSectionId, section.Id);

        }
        await _context.Sections.AddRangeAsync(sectionsToMigrate);
        await _context.SaveChangesAsync();

        return oldNewIds;
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
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
        return section;
    }

    public async Task<bool> IsSectionEditable(Guid id)
    {
        return await _context.Sections.AnyAsync(v => v.Id == id && v.FormVersion.Status == FormVersionStatus.Draft.ToString());
    }

    /// <summary>
    /// Deletes a section with a given Id, throws if no record can be found for a given Id. 
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<Section> DeleteSection(Guid sectionId)
    {
        var sectionToDelete = await _context.Sections
            .Include(s => s.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.QuestionValidation)
            .Include(s => s.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.QuestionOptions)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (sectionToDelete is null)
            throw new RecordNotFoundException(sectionId);

        // should I do RemoveRange explicitly on Pages, Questions?
        _context.Sections.Remove(sectionToDelete);
        await _context.SaveChangesAsync();

        await UpdateSectionOrdering(sectionToDelete.FormVersionId, sectionToDelete.Order);

        return sectionToDelete;
    }

    public async Task<List<Section>> GetSectionsByIdAsync(List<Guid> sectionIds)
    {
        return await _context.Sections.Where(s => sectionIds.Contains(s.Id)).Include(s => s.Pages).ToListAsync();
    }

    private async Task UpdateSectionOrdering(Guid formVersionId, int deletedSectionOrder)
    {
        await _context.Sections
            .Where(sec => sec.FormVersionId == formVersionId && sec.Order > deletedSectionOrder)
            .ExecuteUpdateAsync(s => s.SetProperty(sec => sec.Order, sec => sec.Order - 1));
    }
    
    /// <summary>
    /// Finds a section with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MoveSectionOrderUp(Guid id)
    {
        var modelToUpdate = await _context.Sections.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextHigherModel = await _context.Sections
            .OrderBy(v => v.Order)
            .Where(v => v.Order < modelToUpdate.Order)
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
    /// Finds a section with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> MoveSectionOrderDown(Guid id)
    {
        var modelToUpdate = await _context.Sections.FirstOrDefaultAsync(v => v.Id == id);
        if (modelToUpdate is null)
            throw new RecordNotFoundException(id);

        var nextLowerModel = await _context.Sections
            .OrderByDescending(v => v.Order)
            .Where(v => v.Order > modelToUpdate.Order)
            .FirstOrDefaultAsync();
        if (nextLowerModel is null)
            return true;
        var nextLowest = nextLowerModel.Order;
        nextLowerModel.Order = modelToUpdate.Order;
        modelToUpdate.Order = nextLowest;
        await _context.SaveChangesAsync();

        return true;
    }
}

