using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetNewQualificationsCsvExportResponse
    {
        public IEnumerable<NewQualificationExport> QualificationExports { get; set; } = new List<NewQualificationExport>();
    }
}
