using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.Jobs
{
    public class JobsRepository : IJobsRepository
    {
        private readonly IApplicationDbContext _context;

        public JobsRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Data.Entities.Jobs.Job>> GetJobsAsync()
        {
            return await _context.Jobs
                .ToListAsync();
        }

        public async Task<Entities.Jobs.Job> GetJobByIdAsync(Guid id)
        {
            var record = await _context.Jobs.FirstOrDefaultAsync(v => v.Id == id);
            return record is null ? throw new RecordNotFoundException(id) : record;
        }

        public async Task<Entities.Jobs.Job> GetJobByNameAsync(string name)
        {
            var record = await _context.Jobs.FirstOrDefaultAsync(v => v.Name == name);
            return record is null ? throw new RecordWithNameNotFoundException(name) : record;
        }

        public async Task<bool> UpdateJob(Guid jobId, bool jobEnabled)
        {
            var record = await _context.Jobs.FirstOrDefaultAsync(v => v.Id == jobId) ?? throw new RecordNotFoundException(jobId);
            record.Enabled = jobEnabled;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
