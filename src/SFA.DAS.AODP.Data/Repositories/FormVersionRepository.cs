using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
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

    public async Task<List<FormVersion>> GetLatestFormVersions()
    {
        var all = _context.FormVersions.ToList();

        var top =
            _context.FormVersions
            //.Where(f => !f.Form.Archived)
            .GroupBy(
                t => t.FormId
            )
            .Select(t => new
            {
                FormId = t.Key,
                LatestForm = t.OrderByDescending(x => x.DateCreated).First()
            })
            .AsEnumerable()
            .Select(t => t.LatestForm)
            .ToList();

        return top;
    }

    public async Task<FormVersion?> GetFormVersionByIdAsync(Guid formVersionId)
    {
        return await _context.FormVersions.FirstOrDefaultAsync(v => v.Id == formVersionId);
    }

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

    public async Task<FormVersion?> Update(FormVersion form)
    {
        var formToUpdate = await _context.FormVersions.FirstOrDefaultAsync(v => v.Id == form.Id);
        if (formToUpdate is null)
            return null;
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

    public async Task<bool> Archive(Guid formVersionId)
    {
        var found = await _context.FormVersions.FirstOrDefaultAsync(v => v.Id == formVersionId);
        if (found is null)
            return false;
        found.Status = FormStatus.Archived;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Publish(Guid formVersionId)
    {
        var newPublishedForm = await _context.FormVersions
            .FirstOrDefaultAsync(v => v.Id == formVersionId && v.Status == FormStatus.Draft);
        var oldPublishedForms = await _context.FormVersions
            .Where(v => v.Status == FormStatus.Published)
            .ToListAsync();

        if (newPublishedForm is null) return false;

        newPublishedForm.Status = FormStatus.Published;

        // There shouldn't be multiple published forms, but just in case this is safer 
        foreach (var v in oldPublishedForms)
        {
            v.Status = FormStatus.Archived;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnPublish(Guid formVersionId)
    {
        var form = await _context.FormVersions
            .FirstOrDefaultAsync(v => v.Id == formVersionId);

        if (form is null) return false;

        form.Status = FormStatus.Archived;
        await _context.SaveChangesAsync();
        return true;
    }
}
