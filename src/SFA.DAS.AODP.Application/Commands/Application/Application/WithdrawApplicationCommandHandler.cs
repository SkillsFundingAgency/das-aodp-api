using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application;

public class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand, BaseMediatrResponse<WithdrawApplicationCommandResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMediator _mediator;
    
    public WithdrawApplicationCommandHandler(IApplicationRepository applicationRepository, IMediator mediator)
    {
        _applicationRepository = applicationRepository;
        _mediator = mediator;
    }

    public async Task<BaseMediatrResponse<WithdrawApplicationCommandResponse>> Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<WithdrawApplicationCommandResponse>();

        try
        {
            var application = await _applicationRepository.GetWithReviewFeedbacksByIdAsync(request.ApplicationId);
            if (application.Status == nameof(ApplicationStatus.Withdrawn)) throw new RecordLockedException();

            application.Status = ApplicationStatus.Withdrawn.ToString();
            application.WithdrawnBy = request.WithdrawnBy;
            application.WithdrawnAt = DateTime.UtcNow;
            application.UpdatedAt = DateTime.UtcNow;

            if (application.ApplicationReview?.ApplicationReviewFeedbacks != null)
            {
                var feedbacksToWithdraw = application.ApplicationReview.ApplicationReviewFeedbacks;
                  
                foreach (var feedback in feedbacksToWithdraw)
                {
                    feedback.Status = ApplicationStatus.Withdrawn.ToString();
                }
            }

            await _applicationRepository.UpdateAsync(application);

           var msgResult =  await _mediator.Send(new CreateApplicationMessageCommand()
            {
                ApplicationId = request.ApplicationId,
                MessageText = string.Empty,
                MessageType = MessageType.ApplicationWithdrawn.ToString(),
                SentByEmail = request.WithdrawnByEmail,
                SentByName = request.WithdrawnBy,
                UserType = UserType.AwardingOrganisation.ToString()
            }, cancellationToken);

            if (!msgResult.Success)
            {
                throw new ApplicationMessageException(
                    msgResult.ErrorMessage ?? "Unknown error while creating application message.",
                    msgResult.InnerException
                );
            }
            response.Value = new() { Notifications = msgResult.Value.Notifications };
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
