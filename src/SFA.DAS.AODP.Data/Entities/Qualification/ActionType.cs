namespace SFA.DAS.AODP.Data.Entities.Qualification;

public partial class ActionType
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<QualificationDiscussionHistory> QualificationDiscussionHistories { get; set; } = new List<QualificationDiscussionHistory>();
}