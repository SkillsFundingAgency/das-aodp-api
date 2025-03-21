using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Enum;
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

        public async Task<List<Data.Entities.Jobs.JobRun>> GetJobRunsAsync(string jobName)
        {
            return await _context.JobRuns
                .Include(i => i.Job)
                .Where(w => w.Job.Name == jobName)
                .OrderByDescending(o => o.StartTime)
                .AsNoTracking()
                .Take(10)
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

        public async Task<bool> RequestJobRun(string jobName, string userName)
        {
            var job = await _context.Jobs
                            .Include(i => i.JobRuns)
                            .Where(w => w.Name == jobName)
                            .FirstOrDefaultAsync() ?? throw new RecordWithNameNotFoundException(jobName);

            if (job.Status == JobStatus.Running.ToString())
            {
                return false;
            }

            var lastJobRun = job.JobRuns.OrderByDescending(o => o.StartTime).FirstOrDefault();
            if (lastJobRun != null && lastJobRun.Status == JobStatus.Requested.ToString())
            {
                //already requested a job run
                return true;
            }

            var jobRun = new JobRun() 
            { 
                Id = Guid.NewGuid(),
                JobId = job.Id,
                RecordsProcessed = 0,
                Status = JobStatus.Requested.ToString(),
                User = userName, 
                StartTime = DateTime.Now,
            };
            await _context.JobRuns.AddAsync(jobRun);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
