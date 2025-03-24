using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Message;

public class MarkAllMessagesAsReadCommandHandler : IRequestHandler<MarkAllMessagesAsReadCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IApplicationReviewRepository _reviewRepository;
    private readonly IApplicationReviewFeedbackRepository _feedbackRepository;
    private readonly IApplicationRepository _applicationRepository;

    public MarkAllMessagesAsReadCommandHandler(IApplicationRepository applicationRepository, IApplicationReviewFeedbackRepository feedbackRepository, IApplicationReviewRepository reviewRepository)
    {
        _applicationRepository = applicationRepository;
        _feedbackRepository = feedbackRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(MarkAllMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            if (request.UserType == UserType.AwardingOrganisation.ToString())
            {
                var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
                application.NewMessage = false;
                await _applicationRepository.UpdateAsync(application);
            }
            else
            {
                var review = await _reviewRepository.GetByApplicationIdAsync(request.ApplicationId);
                if (review == null) throw new Exception("No Review record found for the application");
                var feedback = review.ApplicationReviewFeedbacks.First(f => f.Type == request.UserType);
                feedback.NewMessage = false;
                await _feedbackRepository.UpdateAsync(feedback);
            }

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }
        return response;
    }
}