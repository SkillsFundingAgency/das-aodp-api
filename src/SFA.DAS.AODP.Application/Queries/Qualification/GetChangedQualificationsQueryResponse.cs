namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetChangedQualificationsQueryResponse
{
    public List<ChangedQualification> Data { get; set; } = new();

    public class ChangedQualification
    {
        public string QualificationReference { get; set; } = string.Empty;
        public string AwardingOrganisation { get; set; } = string.Empty;
        public string QualificationTitle { get; set; } = string.Empty;
        public string QualificationType { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string AgeGroup { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string SectorSubjectArea { get; set; } = string.Empty;

        public static implicit operator ChangedQualification(Data.Entities.Qualification.ChangedQualification qualification)
        {
            return (new()
            {
                QualificationReference = qualification.QualificationReference,
                AwardingOrganisation = qualification.AwardingOrganisation,
                QualificationTitle = qualification.QualificationTitle,
                QualificationType = qualification.QualificationType,
                Level = qualification.Level,
                AgeGroup = qualification.AgeGroup,
                Subject = qualification.Subject,
                SectorSubjectArea = qualification.SectorSubjectArea,
            });
        }
    }
}
