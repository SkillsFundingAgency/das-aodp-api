using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly IApplicationDbContext _context;
        private readonly ISectionRepository _sectionRepository;

        public FormRepository(IApplicationDbContext context, ISectionRepository sectionRepository)
        {
            _context = context;
            _sectionRepository = sectionRepository;
        }

        public async Task<List<FormVersion>> GetLatestFormVersions()
        {
            var formDb = new Form { Id = Guid.NewGuid(), Archived = true };
            _context.Forms.Add(formDb);

            _context.Forms.Add(new Form { Id = Guid.NewGuid(), Archived = false });
            _context.FormVersions.Add(new()
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.Now,
                FormId = formDb.Id,
                Description = "Something",
                Name = "Name",
                Version = DateTime.Now,
            });
            await _context.SaveChangesAsync();

            // end

            var top =
                _context.FormVersions
                .Where(f => !f.Form.Archived)
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
    }
}
