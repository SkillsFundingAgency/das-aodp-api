namespace SFA.DAS.AODP.Models.Qualifications
{
    public class NewQualificationsFilter
    {
        public string? Name { get; set; }
        public string? Organisation { get; set; }
        public string? QAN { get; set; }
        public List<Guid>? ProcessStatusIds { get; set; }
    }
}
