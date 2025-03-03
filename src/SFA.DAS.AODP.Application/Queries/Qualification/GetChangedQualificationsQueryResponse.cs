using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetChangedQualificationsQueryResponse
    {
        public List<Models.Qualifications.ChangedQualification> ChangedQualifications { get; set; } = new();
    }
}
