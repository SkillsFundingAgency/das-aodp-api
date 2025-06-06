﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[Table("LifecycleStage", Schema = "regulated")]
public partial class LifecycleStage
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public virtual ICollection<QualificationVersions> QualificationVersions { get; set; } = new List<QualificationVersions>();
}