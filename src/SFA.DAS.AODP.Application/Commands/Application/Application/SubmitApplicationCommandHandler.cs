using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

public class SubmitApplicationCommandHandler : IRequestHandler<SubmitApplicationCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IApplicationReviewRepository _applicationReviewRepository;
    private readonly IApplicationReviewFeedbackRepository _applicationReviewFeedbackRepository;
    private readonly IMediator _mediator;

    public SubmitApplicationCommandHandler(IApplicationRepository applicationRepository, IApplicationReviewRepository applicationReviewRepository, IApplicationReviewFeedbackRepository applicationReviewFeedbackRepository, IMediator mediator)
    {
        _applicationRepository = applicationRepository;
        _applicationReviewRepository = applicationReviewRepository;
        _applicationReviewFeedbackRepository = applicationReviewFeedbackRepository;
        _mediator = mediator;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (application.Submitted == true) throw new RecordLockedException();

            var summary = await _applicationRepository.GetSectionSummaryByApplicationIdAsync(request.ApplicationId);
            if (summary.Any(s => s.RemainingPages > 0)) throw new InvalidOperationException("The application has not been completed");


            application.Submitted = true;
            application.Status = ApplicationStatus.InReview.ToString();
            application.SubmittedAt = DateTime.UtcNow;
            application.UpdatedAt = DateTime.UtcNow;


            var review = await _applicationReviewRepository.GetByApplicationIdAsync(request.ApplicationId);
            if (review == null)
            {
                review = new ApplicationReview()
                {
                    ApplicationId = request.ApplicationId,
                };

                await _applicationReviewRepository.CreateAsync(review);

                var qfauReview = new ApplicationReviewFeedback()
                {
                    ApplicationReviewId = review.Id,
                    NewMessage = true,
                    Status = ApplicationStatus.InReview.ToString(),
                    Type = UserType.Qfau.ToString()
                };
                await _applicationReviewFeedbackRepository.CreateAsync(qfauReview);

            }
            else if (review.ApplicationReviewFeedbacks?.Any() == true)
            {
                foreach (var feedback in review.ApplicationReviewFeedbacks)
                {
                    feedback.Status = ApplicationStatus.InReview.ToString();
                    feedback.NewMessage = true;
                }

                await _applicationReviewFeedbackRepository.UpdateAsync(review.ApplicationReviewFeedbacks);
            }

            await _applicationRepository.UpdateAsync(application);

           var msgResult =  await _mediator.Send(new CreateApplicationMessageCommand()
            {
                ApplicationId = request.ApplicationId,
                MessageText = string.Empty,
                MessageType = MessageType.ApplicationSubmitted.ToString(),
                SentByEmail = request.SubmittedByEmail,
                SentByName = request.SubmittedBy,
                UserType = UserType.AwardingOrganisation.ToString()
            });
            if (!msgResult.Success) throw new Exception(msgResult.ErrorMessage, msgResult.InnerException);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.InnerException = ex;
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
