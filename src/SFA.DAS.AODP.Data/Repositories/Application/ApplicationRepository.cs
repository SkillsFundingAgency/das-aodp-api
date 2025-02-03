using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Data.Entities.Application.Application> Create(Data.Entities.Application.Application application)
        {
            application.Id = Guid.NewGuid();
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<Data.Entities.Application.Application> GetApplicationByIdAsync(Guid applicationId)
        {
            var res = await _context.Applications.FirstOrDefaultAsync(v => v.Id == applicationId);
            return res is null ? throw new RecordNotFoundException(applicationId) : res;
        }
    }
}
