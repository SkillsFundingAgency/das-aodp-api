﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[Table("VersionFieldChanges", Schema = "regulated")]
public partial class VersionFieldChange
{
    public Guid Id { get; set; }

    public int? QualificationVersionNumber { get; set; }

    public string? ChangedFieldNames { get; set; }

    public virtual ICollection<QualificationVersions> QualificationVersions { get; set; } = new List<QualificationVersions>();
}