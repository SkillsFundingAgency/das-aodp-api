using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetNewQualificationsQueryResponse
    {
        public List<Models.Qualifications.NewQualification> NewQualifications { get; set; } = new();
    }
}
