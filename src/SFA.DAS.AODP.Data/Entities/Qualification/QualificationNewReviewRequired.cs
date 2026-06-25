namespace SFA.DAS.AODP.Data.Entities.Qualification
{
    public class QualificationNewReviewRequired
    {
        public Guid QualificationId { get; set; }
        public string QualificationReference { get; set; } = "";
        public string AwardingOrganisation { get; set; } = "";
        public string QualificationTitle { get; set; } = "";
        public string QualificationType { get; set; } = "";
        public string? Level { get; set; }
        public bool PreSixteen { get; set; }
        public bool SixteenToEighteen { get; set; }
        public bool EighteenPlus { get; set; }
        public bool NineteenPlus { get; set; }
        public string? Subject { get; set; }
        public string? SectorSubjectArea { get; set; }
        public Guid? ProcessStatusId { get; set; }
        public bool? EligibleForFunding { get; set; }
    }
}


