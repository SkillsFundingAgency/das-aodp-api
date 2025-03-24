namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetDiscussionHistoriesForQualificationQueryResponse
{
    public List<QualificationDiscussionHistory> QualificationDiscussionHistories { get; set; } = new List<QualificationDiscussionHistory>();
    public partial class QualificationDiscussionHistory
    {
        public Guid Id { get; set; }
        public Guid QualificationId { get; set; }
        public Guid ActionTypeId { get; set; }
        public string? UserDisplayName { get; set; }
        public string? Notes { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? Title { get; set; }
        public virtual ActionType ActionType { get; set; } = null!;

        public static implicit operator QualificationDiscussionHistory(Data.Entities.Qualification.QualificationDiscussionHistory entity)
        {
            return new QualificationDiscussionHistory
            {
                Id = entity.Id,
                QualificationId = entity.QualificationId,
                ActionTypeId = entity.ActionTypeId,
                UserDisplayName = entity.UserDisplayName,
                Notes = entity.Notes,
                Timestamp = entity.Timestamp,
                Title = entity.Title,
                ActionType = new ActionType
                {
                    Id = entity.ActionType.Id,
                    Description = entity.ActionType.Description,
                }
            };
        }
    }
    public class ActionType
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
    }


}
