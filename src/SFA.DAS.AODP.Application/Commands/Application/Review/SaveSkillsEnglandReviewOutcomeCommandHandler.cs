using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveSkillsEnglandReviewOutcomeCommandHandler : IRequestHandler<SaveSkillsEnglandReviewOutcomeCommand, BaseMediatrResponse<SaveSkillsEnglandReviewOutcomeCommandResponse>>
    {
        private readonly IApplicationReviewFeedbackRepository _repository;
        private readonly IMediator _mediator;

        public SaveSkillsEnglandReviewOutcomeCommandHandler(IApplicationReviewFeedbackRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<BaseMediatrResponse<SaveSkillsEnglandReviewOutcomeCommandResponse>> Handle(SaveSkillsEnglandReviewOutcomeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<SaveSkillsEnglandReviewOutcomeCommandResponse>();

            try
            {
                var review = await _repository.GeyByReviewIdAndUserType(request.ApplicationReviewId, UserType.SkillsEngland);
                if (!review.ApplicationReview.SharedWithSkillsEngland) throw new InvalidOperationException("The application is not shared with Skills England");

                review.Comments = request.Comments;
                review.Status = request.Approved ? ApplicationStatus.Approved.ToString() : ApplicationStatus.NotApproved.ToString();

                await _repository.UpdateAsync(review);

                var msgResult = await _mediator.Send(new CreateApplicationMessageCommand()
                {
                    ApplicationId = review.ApplicationReview.ApplicationId,
                    MessageText = $"Status: {review.Status} \n Comments: \n {review.Comments}",
                    SentByEmail = request.SentByEmail,
                    SentByName = request.SentByName,
                    UserType = UserType.SkillsEngland.ToString(),
                    MessageType = MessageType.SkillsEnglandFeedbackSubmitted.ToString()

                });
                if (!msgResult.Success) throw new Exception(msgResult.ErrorMessage, msgResult.InnerException);

                response.Value = new() { Notifications = msgResult.Value.Notifications };
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