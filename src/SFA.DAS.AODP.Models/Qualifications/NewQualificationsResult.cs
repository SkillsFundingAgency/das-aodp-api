namespace SFA.DAS.AODP.Models.Qualifications
{
    public class NewQualificationsResult
    {
        public List<NewQualification> Data { get; set; }
        public int TotalRecords { get; set; } = 0;
        public int? Skip { get; set; } = 0;
        public int? Take { get; set; } = 0;
    }
}
