using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.Jobs
{
    public class JobRunsRepository : IJobRunsRepository
    {
        private readonly IApplicationDbContext _context;

        public JobRunsRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Data.Entities.Jobs.JobRun>> GetJobRunsAsync()
        {
            return await _context.JobRuns
                .ToListAsync();
        }

        public async Task<List<Entities.Jobs.JobRun>> GetJobRunsByJobId(Guid jobId)
        {
            var records = await _context.JobRuns
                .Where(v => v.JobId == jobId)
                .ToListAsync();

            if (records.Count == 0)
                throw new RecordNotFoundException(jobId);

            return records;
        }
    }
}
