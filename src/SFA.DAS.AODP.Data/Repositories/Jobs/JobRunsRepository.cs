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

        public async Task<List<Entities.Jobs.JobRun>> GetJobRunsByNameAsync(string name)
        {
            var records = await _context.Jobs
                .Join(_context.JobRuns,
                      job => job.Id,
                      run => run.JobId,
                      (job, run) => new { job, run })
                .Where(x => x.job.Name == name)
                .Select(x => x.run)
                .ToListAsync();

            if (records.Count == 0)
                throw new RecordWithNameNotFoundException(name);
            return records;
        }
    }
}
