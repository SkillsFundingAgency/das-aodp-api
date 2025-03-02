using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetChangedQualificationsQueryResponse
    {
        public List<Models.Qualifications.Qualification> ChangedQualifications { get; set; } = new();
    }
}
