using SFA.DAS.AODP.Data.Entities.Jobs;

namespace SFA.DAS.AODP.Data.Repositories.Jobs
{
    public interface IJobsRepository
    {
        Task<Job> GetJobByIdAsync(Guid id);
        Task<Job> GetJobByNameAsync(string name);
        Task<List<Job>> GetJobsAsync();
        Task<bool> UpdateJob(Guid jobId, bool jobEnabled);
    }
}