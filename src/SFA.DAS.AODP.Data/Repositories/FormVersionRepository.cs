using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Repositories;

public class FormVersionRepository : IFormVersionRepository
{
    private readonly IApplicationDbContext _context;
    private readonly ISectionRepository _sectionRepository;

    public FormVersionRepository(IApplicationDbContext context, ISectionRepository sectionRepository)
    {
        _context = context;
        _sectionRepository = sectionRepository;
    }

    /// <summary>
    /// Returns all the latest form versions for all given forms. 
    /// </summary>
    /// <returns></returns>
    public async Task<List<FormVersion>> GetLatestFormVersions()
    {
        var top =
            _context.FormVersions
            .Where(f => !f.Form.Archived)
            .Where(f => f.Status != FormStatus.Archived)
            .ToList();

        return top;
    }

    /// <summary>
    /// Gets a form version by its DB Id. 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<FormVersion> GetFormVersionByIdAsync(Guid formVersionId)
    {
        var res = await _context.FormVersions.Include(f => f.Sections).FirstOrDefaultAsync(v => v.Id == formVersionId);
        if (res is null)
            throw new RecordNotFoundException(formVersionId);
        return res;
    }

    /// <summary>
    /// Creates a new form and a related form version from a passed in form version. 
    /// </summary>
    /// <param name="formVersionToAdd"></param>
    /// <returns></returns>
    public async Task<FormVersion> Create(FormVersion formVersionToAdd)
    {
        var form = new Form()
        {
            Id = Guid.NewGuid(),
        };
        _context.Forms.Add(form);
        formVersionToAdd.FormId = form.Id;
        formVersionToAdd.DateCreated = DateTime.Now;
        formVersionToAdd.Status = FormStatus.Draft;
        _context.FormVersions.Add(formVersionToAdd);
        await _context.SaveChangesAsync();
        return formVersionToAdd;
    }

    /// <summary>
    /// Updates a given form version using the data and DB Id from a passed in form model. 
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<FormVersion> Update(FormVersion form)
    {
        var formToUpdate = await _context.FormVersions.FirstOrDefaultAsync(v => v.Id == form.Id);
        if (formToUpdate is null)
            throw new RecordNotFoundException(form.Id);
        if (formToUpdate.Status == FormStatus.Published)
        {
            var oldFormId = form.Id;
            form.Status = FormStatus.Draft;
            form.Version = DateTime.Now;
            form.Id = Guid.NewGuid();
            await _sectionRepository.CopySectionsForNewForm(oldFormId, form.Id);
        }
        formToUpdate = form;
        await _context.SaveChangesAsync();
        return formToUpdate;
    }

    /// <summary>
    /// Sets the status of a form version with a given Id to the status of archived. 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> Archive(Guid formVersionId)
    {
        var found = await _context.FormVersions.FirstOrDefaultAsync(v => v.Id == formVersionId);
        if (found is null)
            throw new RecordNotFoundException(formVersionId);
        found.Status = FormStatus.Archived;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Sets the status of a form version with the given ID and with the current status of "Draft" to "Published". 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> Publish(Guid formVersionId)
    {
        var newPublishedForm = await _context.FormVersions
            .FirstOrDefaultAsync(v => v.Id == formVersionId && v.Status == FormStatus.Draft);
        var oldPublishedForms = await _context.FormVersions
            .Where(v => v.Status == FormStatus.Published)
            .ToListAsync();

        if (newPublishedForm is null)
            throw new RecordNotFoundException(formVersionId);

        newPublishedForm.Status = FormStatus.Published;

        // There shouldn't be multiple published forms, but just in case this is safer 
        foreach (var v in oldPublishedForms)
        {
            v.Status = FormStatus.Archived;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Sets the status of a form version with the given ID to "Archived". 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> Unpublish(Guid formVersionId)
    {
        var form = await _context.FormVersions
            .FirstOrDefaultAsync(v => v.Id == formVersionId);

        if (form is null)
            throw new RecordNotFoundException(formVersionId);

        form.Status = FormStatus.Archived;
        await _context.SaveChangesAsync();
        return true;
    }
}
