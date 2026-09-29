namespace SFA.DAS.AODP.Data.Entities.QaaQualification;

public partial class QaaQualificationDownloadLog
{
    public Guid Id { get; set; }
    public string? UserDisplayName { get; set; }
    public DateTime DownloadDate { get; set; }
    public string? FileName { get; set; }
}
