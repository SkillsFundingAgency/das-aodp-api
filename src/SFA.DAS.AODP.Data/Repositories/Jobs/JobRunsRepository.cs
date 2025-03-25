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

        public async Task<Entities.Jobs.JobRun> GetJobRunsById(Guid id)
        {
            var record = await _context.JobRuns
                .Where(v => v.Id == id)
                .FirstOrDefaultAsync();

            if (record == null)
                throw new RecordNotFoundException(id);

            return record;
        }

        public async Task<bool> RequestJobRun(string jobName, string userName)
        {
            var jobRuns = await _context.JobRuns
                            .Include(i => i.Job)
                            .Where(w => w.Job.Name == jobName 
                                    && (w.Status == JobStatus.Running.ToString() 
                                        || w.Status == JobStatus.Requested.ToString() 
                                        || w.Status == JobStatus.RequestSent.ToString()))
                            .FirstOrDefaultAsync();

            if (jobRuns != null)
            {
                // Already running or requested
                return true;
            }            

            var job = await _context.Jobs.FirstOrDefaultAsync(f => f.Name == jobName);

            if (job != null)
            {
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
            else    
            { 
                throw new RecordWithNameNotFoundException(jobName);
            }
        }
    }
}
