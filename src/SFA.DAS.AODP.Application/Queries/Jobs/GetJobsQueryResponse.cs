using SFA.DAS.AODP.Models.Jobs;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{

    public class GetJobsQueryResponse
    {
        public List<Job> Jobs { get; set; } = new();

        public static implicit operator GetJobsQueryResponse(List<Data.Entities.Jobs.Job> jobs)
        {
            GetJobsQueryResponse response = new();

            foreach (var job in jobs)
            {
                response.Jobs.Add(new Job
                {
                    Id = job.Id,
                    Name = job.Name,
                    Enabled = job.Enabled,
                    Status = job.Status,
                    LastRunTime = job.LastRunTime,
                });

            }
            return response;
        }        
    }
}
