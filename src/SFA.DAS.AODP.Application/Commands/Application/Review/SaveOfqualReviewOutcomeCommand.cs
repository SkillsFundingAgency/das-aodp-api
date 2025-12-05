using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveOfqualReviewOutcomeCommand : IRequest<BaseMediatrResponse<SaveOfqualReviewOutcomeCommandResponse>>
    {
        public Guid ApplicationReviewId { get; set; }
        public string? Comments { get; set; }
        public string SentByName { get; set; }
        public string SentByEmail { get; set; }
    }
}