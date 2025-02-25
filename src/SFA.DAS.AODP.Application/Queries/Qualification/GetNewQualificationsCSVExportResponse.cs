using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetNewQualificationsCsvExportResponse
    {
        public List<QualificationExport> QualificationExports { get; set; } = new();
    }
}
