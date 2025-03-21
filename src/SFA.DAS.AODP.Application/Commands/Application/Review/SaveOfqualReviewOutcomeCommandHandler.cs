using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveOfqualReviewOutcomeCommandHandler : IRequestHandler<SaveOfqualReviewOutcomeCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFeedbackRepository _repository;
        private readonly IMediator _mediator;

        public SaveOfqualReviewOutcomeCommandHandler(IApplicationReviewFeedbackRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveOfqualReviewOutcomeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _repository.GeyByReviewIdAndUserType(request.ApplicationReviewId, UserType.Ofqual);
                if (!review.ApplicationReview.SharedWithOfqual) throw new InvalidOperationException("The application is not shared with Ofqual");

                review.Comments = request.Comments;
                review.Status = ApplicationStatus.Reviewed.ToString();

                await _repository.UpdateAsync(review);

                var msgResult = await _mediator.Send(new CreateApplicationMessageCommand()
                {
                    ApplicationId = review.ApplicationReview.ApplicationId,
                    MessageText = review.Comments ?? string.Empty,
                    SentByEmail = request.SentByEmail,
                    SentByName = request.SentByName,
                    UserType = UserType.Ofqual.ToString(),
                    MessageType = MessageType.OfqualFeedbackSubmitted.ToString()

                });
                if (!msgResult.Success) throw new Exception(msgResult.ErrorMessage, msgResult.InnerException);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
                response.Success = false;
            }
            return response;
        }
    }
}