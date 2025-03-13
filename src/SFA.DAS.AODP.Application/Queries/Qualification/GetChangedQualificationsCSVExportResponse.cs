using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetChangedQualificationsCsvExportResponse
    {
        public List<ChangedQualificationExport> QualificationExports { get; set; } = new();
    }
}
