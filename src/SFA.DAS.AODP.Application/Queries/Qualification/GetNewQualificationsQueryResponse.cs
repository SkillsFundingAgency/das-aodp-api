using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetNewQualificationsQueryResponse
    {
        public List<NewQualification> Data { get; set; }
        public int TotalRecords { get; set; } = 0;
        public int? Skip { get; set; } = 0;
        public int? Take { get; set; } = 0;
    }
}
