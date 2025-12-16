namespace SFA.DAS.AODP.Data.Entities.Qualification;
public partial class QualificationSearchResultDto
{
    public Guid Id { get; set; }
    public string Qan { get; set; } = null!;
    public string? QualificationName { get; set; }

    public float? Score;
}