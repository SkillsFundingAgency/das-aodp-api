namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class Survey
    {
        public Guid Id { get; set; }
        public string Page { get; set; }
        public int SatisfactionScore { get; set; }
        public string Comments { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
