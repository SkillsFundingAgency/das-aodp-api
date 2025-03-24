
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;

public class GetProcessingStatusesQueryResponse
{
    public List<ProcessStatus> ProcessStatuses { get; set; } = new List<ProcessStatus>();

    public class ProcessStatus
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? IsOutcomeDecision { get; set; }

        public static implicit operator ProcessStatus(Data.Entities.Qualification.ProcessStatus entity)
        {
            return new ProcessStatus
            {
                Id = entity.Id,
                Name = entity.Name,
                IsOutcomeDecision = entity.IsOutcomeDecision,
            };
        }
    }
}
