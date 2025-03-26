using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveReviewOwnerUpdateCommandHandler : IRequestHandler<SaveReviewOwnerUpdateCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFeedbackRepository _repository;
        private readonly IMediator _mediator;

        public SaveReviewOwnerUpdateCommandHandler(IApplicationReviewFeedbackRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveReviewOwnerUpdateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                if (!Enum.TryParse(request.UserType, true, out UserType userType))
                {
                    throw new ArgumentException($"Invalid User Type: {request.UserType}");
                }

                var review = await _repository.GeyByReviewIdAndUserType(request.ApplicationReviewId, userType);
                if (userType == UserType.SkillsEngland && !review.ApplicationReview.SharedWithSkillsEngland) throw new InvalidOperationException("The application is not shared with Skills England");
                if (userType == UserType.Ofqual && !review.ApplicationReview.SharedWithOfqual) throw new InvalidOperationException("The application is not shared with Ofqual");

                string previousOwner = review.Owner ?? "N/A";
                review.Owner = request.Owner;

                await _repository.UpdateAsync(review);

                var msgCommand = new CreateApplicationMessageCommand()
                {
                    ApplicationId = review.ApplicationReview.ApplicationId,
                    MessageText = $"Previous owner: {previousOwner}\nNew owner: {request.Owner}",
                    SentByEmail = request.SentByEmail,
                    SentByName = request.SentByName,
                    UserType = userType.ToString(),
                };

                switch (userType)
                {
                    case UserType.SkillsEngland:
                        msgCommand.MessageType = MessageType.SkillsEnglandOwnerUpdated.ToString();
                        break;
                    case UserType.Ofqual:
                        msgCommand.MessageType = MessageType.OfqualOwnerUpdated.ToString();
                        break;
                    case UserType.Qfau:
                        msgCommand.MessageType = MessageType.QfauOwnerUpdated.ToString();
                        break;
                }
                var msgResult = await _mediator.Send(msgCommand);
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