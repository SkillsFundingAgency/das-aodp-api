using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewOutcomeCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid ApplicationReviewId { get; set; }
        public string? Comments { get; set; }
        public bool Approved { get; set; }
    }
}