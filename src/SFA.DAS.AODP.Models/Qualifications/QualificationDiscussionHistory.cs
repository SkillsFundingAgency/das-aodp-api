namespace SFA.DAS.AODP.Models.Qualifications;

public class QualificationDiscussionHistory
{
    public Guid Id { get; set; }
    public Guid QualificationId { get; set; }
    public Guid ActionTypeId { get; set; }
    public string? UserDisplayName { get; set; }
    public string? Notes { get; set; }
    public DateTime? Timestamp { get; set; }
    public virtual ActionType ActionType { get; set; } = null!;
}

public class ActionType
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
}
