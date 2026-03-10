using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[ExcludeFromCodeCoverage]
[Table("RolloverWorkflowRunFundingOffer")]
public class RolloverWorkflowRunFundingOffer
{
    public Guid Id { get; private set; }

    public Guid RolloverWorkflowRunId { get; private set; }

    public Guid FundingOfferId { get; private set; }

    public virtual RolloverWorkflowRun WorkflowRun { get; private set; } = null!;
}