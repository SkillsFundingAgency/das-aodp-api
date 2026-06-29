namespace SFA.DAS.AODP.Models.Rollover;

public class RolloverQualificationVersion
{
    public Guid Id { get; set; }
    public string? QualificationReference { get; set; }
    public string? QualificationName { get; set; }
    public Guid AwardingOrganisationId { get; set; }
}
