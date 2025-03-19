using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetChangedQualificationsCsvExportResponse
    {
        public IEnumerable<ChangedQualificationExport> QualificationExports { get; set; } = new List<ChangedQualificationExport>();
    }
}
