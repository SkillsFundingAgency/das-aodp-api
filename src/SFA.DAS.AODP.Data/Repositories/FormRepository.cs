using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly IApplicationDbContext _context;

        public FormRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FormVersion>> GetLatestFormVersions()
        {
            var formDb = new Form { Id = Guid.NewGuid(), IsActive = true };
            _context.Forms.Add(formDb);

            _context.Forms.Add(new Form { Id = Guid.NewGuid(), IsActive = false });
            _context.FormVersions.Add(new()
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.Now,
                FormId = formDb.Id,
                Description = "Something",
                Name = "Name",
                Version = "123"
            });
            await _context.SaveChangesAsync();

            // end

            var top =
                _context.FormVersions
                .Where(f => f.Form.IsActive)
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
    }
}
