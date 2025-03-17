namespace SFA.DAS.AODP.Data.Repositories.Jobs
{
    public interface IJobRunsRepository
    {
        Task<List<Data.Entities.Jobs.JobRun>> GetJobRunsAsync();

        Task<List<Entities.Jobs.JobRun>> GetJobRunsByJobId(Guid jobId);

        Task<List<Entities.Jobs.JobRun>> GetJobRunsByNameAsync(string name);

    }
}
