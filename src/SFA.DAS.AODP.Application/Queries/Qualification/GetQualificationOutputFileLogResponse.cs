using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;

public class GetQualificationOutputFileLogResponse
{
    public IEnumerable<QualificationOutputFileLog> OutputFileLogs { get; set; } = new List<QualificationOutputFileLog>();
}
