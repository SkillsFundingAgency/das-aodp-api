
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;

public class GetQualificationOutputFileResponse
{
    public string FileName { get; set; } = string.Empty;

    public byte[] ZipFileContent { get; set; } = Array.Empty<byte>();

}
