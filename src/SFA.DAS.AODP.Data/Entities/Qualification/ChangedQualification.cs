﻿
namespace SFA.DAS.AODP.Data.Entities.Qualification;

public class ChangedQualification
{
    public string? QualificationReference { get; set; } = string.Empty;
    public string? AwardingOrganisation { get; set; } = string.Empty;
    public string? QualificationTitle { get; set; } = string.Empty;
    public string? QualificationType { get; set; } = string.Empty;
    public string? Level { get; set; } = string.Empty;
    public string? AgeGroup { get; set; } = string.Empty;
    public string? Subject { get; set; } = string.Empty;
    public string? SectorSubjectArea { get; set; } = string.Empty;
    public string? ChangedFieldNames { get; set; }
    public string? Status { get; set; }
    public Guid? ProcessStatusId { get; set; }
}
