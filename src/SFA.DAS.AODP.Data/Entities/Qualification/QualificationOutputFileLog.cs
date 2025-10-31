namespace SFA.DAS.AODP.Data.Entities.Qualification;

public partial class QualificationOutputFileLog
{
    public Guid Id { get; set; }
    public string? UserDisplayName { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? ApprovedFileName { get; set; }
    public string? ArchivedFileName { get; set; }
}