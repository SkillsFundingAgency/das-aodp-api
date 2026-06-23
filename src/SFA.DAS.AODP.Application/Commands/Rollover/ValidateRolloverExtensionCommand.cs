using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    [ExcludeFromCodeCoverage]
    public class ValidateRolloverExtensionCommand : IRequest<BaseMediatrResponse<ValidateRolloverExtensionCommandResponse>>
    {
        public List<RolloverCandidateForValidation> RolloverCandidates { get; set; } = new();
    }

    [ExcludeFromCodeCoverage]
    public class RolloverCandidateForValidation
    {
        public int RowNumber { get; set; }
        public required string Qan { get; set; }
        public required string FundingStreamName { get; set; }
        public required string RollOverStatus { get; set; }
        public string? ExclusionReason { get; set; }
        public required DateTime ProposedFundingApprovalEndDate { get; set; }
        public string? Comments { get; init; }
    }
}