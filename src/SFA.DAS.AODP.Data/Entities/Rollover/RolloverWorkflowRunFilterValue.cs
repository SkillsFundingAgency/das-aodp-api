using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[ExcludeFromCodeCoverage]
[Table("RolloverWorkflowRunFilterValue")]
public class RolloverWorkflowRunFilterValue
{
    public Guid Id { get; private set; }

    public Guid RolloverWorkflowRunFilterId { get; private set; }

    public string? ValueText { get; private set; }
    
    public Guid? ValueGuid { get; private set; }

    public int? ValueInt { get; private set; }

    public string? DisplayText { get; private set; }

    public virtual RolloverWorkflowRunFilter RolloverWorkflowRunFilter { get; private set; } = null!;
}