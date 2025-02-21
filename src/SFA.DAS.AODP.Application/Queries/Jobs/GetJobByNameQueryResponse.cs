using SFA.DAS.AODP.Models.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{

    public class GetJobByNameQueryResponse
    {
        public Job Job { get; set; } = new();

        public static implicit operator GetJobByNameQueryResponse(Data.Entities.Jobs.Job job)
        {
            GetJobByNameQueryResponse response = new();
           
                response.Job = new Job
                {
                    Id = job.Id,
                    Name = job.Name,
                    Enabled = job.Enabled,
                    Status = job.Status,
                    LastRunTime = job.LastRunTime,
                };

            return response;
        }        
    }
}
