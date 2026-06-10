using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.Application
{
    [ExcludeFromCodeCoverage]
    public class ApplicationExportMetadata
    {
        public Guid ApplicationId { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string QAN { get; set; } = string.Empty;
        public string QualificationTitle { get; set; } = string.Empty;
        public string FormName { get; set; } = string.Empty;
        public int SubmissionId { get; set; }
    }
}
