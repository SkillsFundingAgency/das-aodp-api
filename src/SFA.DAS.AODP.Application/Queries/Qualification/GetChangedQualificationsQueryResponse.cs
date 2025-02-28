using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetChangedQualificationsQueryResponse
{
    public List<Qualification> Data { get; set; } = new();

    public class Qualification
    {
        public Guid Id { get; set; }
        public DateTime DateOfOfqualDataSnapshot { get; set; }
        public Guid QualificationId { get; set; }
        public Guid AwardingOrganisationId { get; set; }
        public string Level { get; set; } = string.Empty;
        public string QualificationType { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public string SectorSubjectArea { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AwardingOrganisationUrl { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }

        public static implicit operator Qualification(Data.Entities.Qualification.Qualification qualification)
        {
            return (new()
            {
                Id = qualification.Id,
                DateOfOfqualDataSnapshot = qualification.DateOfOfqualDataSnapshot,
                QualificationId = qualification.QualificationId,
                AwardingOrganisationId = qualification.AwardingOrganisationId,
                Level = qualification.Level,
                QualificationType = qualification.QualificationType,
                SubCategory = qualification.SubCategory,
                SectorSubjectArea = qualification.SectorSubjectArea,
                Status = qualification.Status,
                AwardingOrganisationUrl = qualification.AwardingOrganisationUrl,
                ImportDate = qualification.ImportDate,
            });
        }
    }
}
