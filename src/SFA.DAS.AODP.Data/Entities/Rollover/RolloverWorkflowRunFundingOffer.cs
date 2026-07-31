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

    public static RolloverWorkflowRunFundingOffer Create(
        Guid rolloverWorkflowRunId,
        Guid fundingOfferId)
    {
        return new RolloverWorkflowRunFundingOffer
        {
            Id = Guid.NewGuid(),
            RolloverWorkflowRunId = rolloverWorkflowRunId,
            FundingOfferId = fundingOfferId
        };
    }
}