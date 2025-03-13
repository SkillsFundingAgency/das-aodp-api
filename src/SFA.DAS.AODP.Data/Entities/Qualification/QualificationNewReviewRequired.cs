namespace SFA.DAS.AODP.Data.Entities.Qualification
{
    public class QualificationNewReviewRequired
    {
        //public Guid? QualificationVersionId { get; set; }
        public string QualificationReference { get; set; } = "";
        public string AwardingOrganisation { get; set; } = "";
        public string QualificationTitle { get; set; } = "";
        public string QualificationType { get; set; } = "";
        public string? Level { get; set; }
        public string? AgeGroup { get; set; }
        public string? Subject { get; set; }
        public string? SectorSubjectArea { get; set; }
    }
}


