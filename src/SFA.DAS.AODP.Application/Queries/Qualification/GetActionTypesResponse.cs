
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;

public class GetActionTypesResponse
{
    public List<ActionType> ActionTypes { get; set; } = new List<ActionType>();

    public class ActionType
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
    }
}
