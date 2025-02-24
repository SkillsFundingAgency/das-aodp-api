using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetNewQualificationsCSVExportResponse
    {
        public List<QualificationExport> QualificationExports { get; set; } = new();
    }
}
