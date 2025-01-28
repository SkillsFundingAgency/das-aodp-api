using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Infrastructure.Context;
using SFA.DAS.AODP.Models.Archived.Forms.FormBuilder;
using SFA.DAS.AODP.Models.Form;

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
            await _context.FormVersions
            .Where(f => f.Form.Status != FormStatus.Deleted.ToString())
            .Where(f => f.Status != FormVersionStatus.Archived.ToString())
            .ToListAsync();

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
        return res is null ? throw new RecordNotFoundException(formVersionId) : res;
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
            Status = FormStatus.Active.ToString()
        };
        _context.Forms.Add(form);

        formVersionToAdd.Form = form;
        formVersionToAdd.FormId = form.Id;
        formVersionToAdd.DateCreated = DateTime.Now;
        formVersionToAdd.Status = FormVersionStatus.Draft.ToString();
        formVersionToAdd.Version = DateTime.UtcNow;

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
        _context.FormVersions.Update(form);
        await _context.SaveChangesAsync();
        return form;
    }

    /// <summary>
    /// Sets the status of a form version with a given Id to the status of archived. 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<bool> Archive(Guid formVersionId)
    {
        var found = await _context.FormVersions.FirstOrDefaultAsync(v => v.Id == formVersionId) ?? throw new RecordNotFoundException(formVersionId);
        found.Status = FormVersionStatus.Archived.ToString();
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
            .FirstOrDefaultAsync(v => v.Id == formVersionId);

        var oldPublishedForms = await _context.FormVersions
            .Where(v => v.Status == FormVersionStatus.Published.ToString())
            .ToListAsync();

        if (newPublishedForm is null)
            throw new RecordNotFoundException(formVersionId);

        newPublishedForm.Status = FormVersionStatus.Published.ToString();

        // There shouldn't be multiple published forms, but just in case this is safer 
        foreach (var v in oldPublishedForms)
        {
            v.Status = FormVersionStatus.Archived.ToString();
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
            .FirstOrDefaultAsync(v => v.Id == formVersionId) ?? throw new RecordNotFoundException(formVersionId);
        form.Status = FormVersionStatus.Archived.ToString();
        await _context.SaveChangesAsync();
        return true;
    }
}
