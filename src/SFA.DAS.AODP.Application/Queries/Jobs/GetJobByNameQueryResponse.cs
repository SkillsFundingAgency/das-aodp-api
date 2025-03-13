using SFA.DAS.AODP.Models.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{

    public class GetJobByNameQueryResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public bool Enabled { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? LastRunTime { get; set; }

        public static implicit operator GetJobByNameQueryResponse(Data.Entities.Jobs.Job job)
        {
            GetJobByNameQueryResponse response = new()           
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
