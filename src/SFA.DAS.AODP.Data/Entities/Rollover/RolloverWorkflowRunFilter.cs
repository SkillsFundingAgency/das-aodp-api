using System.ComponentModel.DataAnnotations.Schema;
using SFA.DAS.AODP.Data.Entities.Rollover.Enums;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[Table("RolloverWorkflowRunFilter")]
public class RolloverWorkflowRunFilter
{
    public Guid Id { get; private set; }

    public Guid RolloverWorkflowRunId { get; private set; }

    public FilterKey FilterKey { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    public virtual RolloverWorkflowRun WorkflowRun { get; private set; } = null!;

    public virtual IReadOnlyCollection<RolloverWorkflowRunFilterValue> Values => _values;
    private readonly List<RolloverWorkflowRunFilterValue> _values = new();

    public static RolloverWorkflowRunFilter Create(Guid workflowRunId, FilterKey key, DateTime createdAt)
        => new()
        {
            RolloverWorkflowRunId = workflowRunId,
            FilterKey = key,
            CreatedAt = createdAt
        };
}