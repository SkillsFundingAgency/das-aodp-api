using System.Diagnostics.CodeAnalysis;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Entities.QaaQualification;

[ExcludeFromCodeCoverage]
public partial record QaaQualificationDiscussionHistory
{
    public Guid Id { get; set; }
    public Guid QaaQualificationId { get; set; }
    public Guid ActionTypeId { get; set; }
    public string? UserDisplayName { get; set; }
    public string? Notes { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? Title { get; set; }
    public virtual ActionType ActionType { get; set; } = null!;
    public virtual RegulatedQaaQualification QaaQualification { get; set; } = null!;
}
