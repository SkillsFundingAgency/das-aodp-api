namespace SFA.DAS.AODP.Data.Repositories.Jobs
{
    public interface IJobRunsRepository
    {
        Task<List<Data.Entities.Jobs.JobRun>> GetJobRunsAsync(string jobName);

        Task<List<Entities.Jobs.JobRun>> GetJobRunsByJobId(Guid jobId);
        Task<bool> RequestJobRun(string jobName, string userName);
    }
}
