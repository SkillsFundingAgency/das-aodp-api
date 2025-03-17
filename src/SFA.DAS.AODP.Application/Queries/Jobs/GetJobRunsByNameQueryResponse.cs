using SFA.DAS.AODP.Models.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobRunsByNameQueryResponse
    {
        public List<JobRun> JobRuns { get; set; } = new();

        public static implicit operator GetJobRunsByNameQueryResponse(List<Data.Entities.Jobs.JobRun> jobRuns)
        {
            GetJobRunsByNameQueryResponse response = new();

            foreach (var jobRun in jobRuns)
            {
                response.JobRuns.Add(new JobRun
                {
                    Id = jobRun.Id,
                    Status = jobRun.Status,
                    StartTime = jobRun.StartTime,
                    EndTime = jobRun.EndTime,
                    User = jobRun.User,
                    RecordsProcessed = jobRun.RecordsProcessed,
                    JobId = jobRun.JobId,
                });

            }
            return response;
        }
    }
}
