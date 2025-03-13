using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class UpdateApplicationReviewSharingHandler : IRequestHandler<UpdateApplicationReviewSharingCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewRepository _applicationReviewRepository;
        private readonly IApplicationReviewFeedbackRepository _applicationReviewFeedbackRepository;
        private readonly IMediator _mediator;

        public UpdateApplicationReviewSharingHandler(IApplicationReviewRepository applicationReviewRepository, IApplicationReviewFeedbackRepository applicationReviewFeedbackRepository, IMediator mediator)
        {
            _applicationReviewRepository = applicationReviewRepository;
            _applicationReviewFeedbackRepository = applicationReviewFeedbackRepository;
            _mediator = mediator;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateApplicationReviewSharingCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _applicationReviewRepository.GetByIdAsync(request.ApplicationReviewId);

                if (request.ApplicationReviewUserType == UserType.Ofqual.ToString())
                {
                    review.SharedWithOfqual = request.ShareApplication;
                }
                else if (request.ApplicationReviewUserType == UserType.SkillsEngland.ToString())
                {
                    review.SharedWithSkillsEngland = request.ShareApplication;
                }

                await _applicationReviewRepository.UpdateAsync(review);

                if (!review.ApplicationReviewFeedbacks.Exists(f => f.Type == request.ApplicationReviewUserType.ToString()))
                {
                    await _applicationReviewFeedbackRepository.CreateAsync(new ApplicationReviewFeedback()
                    {
                        ApplicationReviewId = review.Id,
                        Type = request.ApplicationReviewUserType.ToString(),
                        NewMessage = true,
                        Status = ApplicationStatus.InReview.ToString()
                    });
                }

                CreateApplicationMessageCommand msgCommand = BuildMessageCommand(request, review);
                var msgResult = await _mediator.Send(msgCommand, cancellationToken);
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

        private static CreateApplicationMessageCommand BuildMessageCommand(UpdateApplicationReviewSharingCommand request, ApplicationReview review)
        {
            var msgCommand = new CreateApplicationMessageCommand()
            {
                ApplicationId = review.ApplicationId,
                SentByEmail = request.SentByEmail,
                SentByName = request.SentByName,
                UserType = request.UserType,
                MessageText = string.Empty
            };

            if (request.ShareApplication && request.ApplicationReviewUserType == UserType.Ofqual.ToString())
            {
                msgCommand.MessageType = MessageType.ApplicationSharedWithOfqual.ToString();
            }
            else if (!request.ShareApplication && request.ApplicationReviewUserType == UserType.Ofqual.ToString())
            {
                msgCommand.MessageType = MessageType.ApplicationUnsharedWithOfqual.ToString();
            }

            else if (request.ShareApplication && request.ApplicationReviewUserType == UserType.SkillsEngland.ToString())
            {
                msgCommand.MessageType = MessageType.ApplicationSharedWithSkillsEngland.ToString();
            }
            else if (!request.ShareApplication && request.ApplicationReviewUserType == UserType.SkillsEngland.ToString())
            {
                msgCommand.MessageType = MessageType.ApplicationUnsharedWithSkillsEngland.ToString();
            }

            return msgCommand;
        }
    }
}