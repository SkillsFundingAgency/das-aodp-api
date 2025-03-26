using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewDecisionCommandHandler : IRequestHandler<SaveQfauFundingReviewDecisionCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFeedbackRepository _reviewRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMediator _mediator;
        public SaveQfauFundingReviewDecisionCommandHandler(IApplicationReviewFeedbackRepository repository, IMediator mediator, IApplicationRepository applicationRepository)
        {
            _reviewRepository = repository;
            _mediator = mediator;
            _applicationRepository = applicationRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQfauFundingReviewDecisionCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _reviewRepository.GeyByReviewIdAndUserType(request.ApplicationReviewId, Models.Application.UserType.Qfau);

                var msg = new CreateApplicationMessageCommand()
                {
                    ApplicationId = review.ApplicationReview.ApplicationId,
                    MessageType = MessageType.AoInformedOfDecision.ToString(),
                    UserType = UserType.Qfau.ToString(),
                    SentByEmail = request.SentByEmail,
                    SentByName = request.SentByName,
                };

                StringBuilder msgText = new();

                msgText.AppendLine("Feedback from DfE:");
                msgText.AppendLine(review.Comments);
                msgText.AppendLine();
                if (review.ApplicationReview.ApplicationReviewFundings.Any())
                {
                    msgText.AppendLine("The following offers have been approved:");
                    msgText.AppendLine();

                    foreach (var offer in review.ApplicationReview.ApplicationReviewFundings)
                    {
                        msgText.AppendLine($"Offer: {offer.FundingOffer.Name}");
                        msgText.AppendLine($"Start date: {offer.StartDate}");
                        msgText.AppendLine($"End date: {offer.EndDate}");
                        msgText.AppendLine($"Comments: {offer.Comments}");
                        msgText.AppendLine();
                    }
                }

                msg.MessageText = msgText.ToString();

                var msgResult = await _mediator.Send(msg);
                if (!msgResult.Success) throw new Exception(msgResult.ErrorMessage, msgResult.InnerException);

                review.LatestCommunicatedToAwardingOrganisation = true;
                await _reviewRepository.UpdateAsync(review);

                var application = await _applicationRepository.GetByIdAsync(review.ApplicationReview.ApplicationId);
                application.Status = review.Status;
                await _applicationRepository.UpdateAsync(application);

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