namespace SFA.DAS.AODP.Data.Entities.Qualification;

public class SearchedQualification
{
    public Guid Id { get; set; }
    public string Qan { get; set; } = null!;
    public string? QualificationName { get; set; }
    public Guid? Status { get; set; }
}
