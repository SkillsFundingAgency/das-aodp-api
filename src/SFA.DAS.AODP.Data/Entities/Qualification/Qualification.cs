using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

public partial class Qualification
{
    public Guid Id { get; set; }
    public string Qan { get; set; } = null!;
    public string? QualificationName { get; set; }
    public virtual ICollection<FundedQualification> Qualifications { get; set; } = new List<FundedQualification>();
    public virtual ICollection<QualificationDiscussionHistory> QualificationDiscussionHistories { get; set; } = new List<QualificationDiscussionHistory>();
    public virtual ICollection<QualificationVersions> QualificationVersions { get; set; } = new List<QualificationVersions>();
}