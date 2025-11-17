namespace SFA.DAS.AODP.Application.Queries.Qualifications;

public class GetQualificationOutputFileResponse
{
    public string FileName { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "text/csv";
}
