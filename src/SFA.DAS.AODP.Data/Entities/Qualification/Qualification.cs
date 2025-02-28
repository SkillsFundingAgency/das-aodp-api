﻿
namespace SFA.DAS.AODP.Data.Entities.Qualification;

public class Qualification
{
    public Guid Id { get; set; }
    public DateTime DateOfOfqualDataSnapshot { get; set; }
    public Guid QualificationId { get; set; }
    public Guid AwardingOrganisationId { get; set; }
    public string Level { get; set; } = string.Empty;
    public string QualificationType { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string SectorSubjectArea { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AwardingOrganisationUrl { get; set; } = string.Empty;
    public DateTime ImportDate { get; set; }
    public List<QualificationVersion> QualificationVersions { get; set; } = new();
}
