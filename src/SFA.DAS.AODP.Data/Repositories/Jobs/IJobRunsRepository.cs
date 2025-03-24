namespace SFA.DAS.AODP.Data.Repositories.Jobs
{
    public interface IJobRunsRepository
    {
        Task<List<Data.Entities.Jobs.JobRun>> GetJobRunsAsync(string jobName);
        Task<Entities.Jobs.JobRun> GetJobRunsById(Guid id);
        Task<bool> RequestJobRun(string jobName, string userName);
    }
}
