using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;


public partial class GetChangedQualificationsQueryResponse
{
    public List<SFA.DAS.AODP.Models.Qualifications.ChangedQualification> Data { get; set; } = new();
    public int TotalRecords { get; set; } = 0;
    public int? Skip { get; set; } = 0;
    public int? Take { get; set; } = 0;
}
