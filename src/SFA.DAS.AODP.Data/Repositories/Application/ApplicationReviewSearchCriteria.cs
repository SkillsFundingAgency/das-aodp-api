using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationReviewSearchCriteria
    {
        public required UserType ReviewType { get; init; }
        public int Offset { get; init; } = 0;
        public int Limit { get; init; } = 10;
        public bool IncludeApplicationWithNewMessages { get; init; }
        public IReadOnlyCollection<string>? ApplicationStatuses { get; init; }
        public string? ApplicationSearch { get; init; }
        public string? AwardingOrganisationSearch { get; init; }
        public string? ReviewerSearch { get; init; }
        public bool UnassignedOnly { get; init; }
    }
}
