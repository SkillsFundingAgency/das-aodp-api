namespace SFA.DAS.AODP.Data.Entities.Qualification;

public partial class QualificationOutputFileLog
{
    public Guid Id { get; set; }
    public string? UserDisplayName { get; set; }
    public DateTime DownloadDate { get; set; }
    public DateTime PublicationDate { get; set; }
    public string? FileName { get; set; }
}