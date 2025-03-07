namespace SFA.DAS.AODP.Models.Qualifications
{
    public class ChangedQualificationsResult
    {
        public List<ChangedQualification> Data { get; set; }
        public int TotalRecords { get; set; } = 0;
        public int? Skip { get; set; } = 0;
        public int? Take { get; set; } = 0;
    }
}
