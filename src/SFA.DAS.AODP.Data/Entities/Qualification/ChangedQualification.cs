
namespace SFA.DAS.AODP.Data.Entities.Qualification;

public class ChangedQualification
{
    public Guid Id { get; set; }
    public Guid Qan { get; set; }
    public string QualificationName { get; set; } = string.Empty;
    public List<QualificationVersion> QualificationVersions { get; set; } = new();
}
