using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Message;

public class CreateApplicationMessageCommandHandler : IRequestHandler<CreateApplicationMessageCommand, BaseMediatrResponse<CreateApplicationMessageCommandResponse>>
{
    private readonly IApplicationReviewRepository _reviewRepository;
    private readonly IApplicationReviewFeedbackRepository _feedbackRepository;
    private readonly IApplicationMessagesRepository _messageRepository;
    private readonly IApplicationRepository _applicationRepository;

    public CreateApplicationMessageCommandHandler(IApplicationMessagesRepository messageRepository, IApplicationRepository applicationRepository, IApplicationReviewFeedbackRepository feedbackRepository, IApplicationReviewRepository reviewRepository)
    {
        _messageRepository = messageRepository;
        _applicationRepository = applicationRepository;
        _feedbackRepository = feedbackRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<BaseMediatrResponse<CreateApplicationMessageCommandResponse>> Handle(CreateApplicationMessageCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateApplicationMessageCommandResponse>();

        try
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            application.UpdatedAt = DateTime.UtcNow;

            if (!Enum.TryParse(request.UserType, true, out UserType userType))
                throw new ArgumentException($"Invalid User Type: {request.UserType}");

            if (!Enum.TryParse(request.MessageType, true, out MessageType messageType))
                throw new ArgumentException($"Invalid Message Type: {request.MessageType}");

            var messageTypeConfiguration = MessageTypeConfigurationRules.GetMessageSharingSettings(messageType);

            var canUserCreateMessage = messageTypeConfiguration.AvailableTo.Contains(userType);

            if (!canUserCreateMessage)
                throw new ArgumentException($"User of type {request.UserType} cannot create message type of {request.MessageType}");

            var messageId = await _messageRepository.CreateAsync(new()
            {
                ApplicationId = request.ApplicationId,
                Text = request.MessageText,
                Type = messageType,
                MessageHeader = messageTypeConfiguration.MessageHeader,
                SharedWithDfe = messageTypeConfiguration.SharedWithDfe,
                SharedWithOfqual = messageTypeConfiguration.SharedWithOfqual,
                SharedWithSkillsEngland = messageTypeConfiguration.SharedWithSkillsEngland,
                SharedWithAwardingOrganisation = messageTypeConfiguration.SharedWithAwardingOrganisation,
                SentByName = request.SentByName,
                SentByEmail = request.SentByEmail
            });

            HandleActionMessages(application, messageType);
            await HandleReviewUpdate(application.Id, messageTypeConfiguration, messageType);
            await _applicationRepository.UpdateAsync(application);

            response.Value = new() { Id = messageId };
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

    private async Task HandleReviewUpdate(Guid applicationId, MessageTypeConfiguration messageTypeConfiguration, MessageType messageType)
    {
        var review = await _reviewRepository.GetByApplicationIdAsync(applicationId);
        if (review == null) return;

        var feedbacksToUpdate = new List<ApplicationReviewFeedback>();
        foreach (var feedback in review.ApplicationReviewFeedbacks ?? [])
        {
            bool updateRequired = false;
            if (feedback.Type == UserType.Ofqual.ToString() && messageTypeConfiguration.SharedWithOfqual)
            {
                updateRequired = true;
            }
            else if (feedback.Type == UserType.SkillsEngland.ToString() && messageTypeConfiguration.SharedWithSkillsEngland)
            {
                updateRequired = true;
            }
            else if (feedback.Type == UserType.Qfau.ToString() && messageTypeConfiguration.SharedWithDfe)
            {
                updateRequired = true;
            }

            if (updateRequired)
            {
                feedback.NewMessage = true;
                feedbacksToUpdate.Add(feedback);
            }
        }

        var qfauReview = review.ApplicationReviewFeedbacks?.FirstOrDefault(f => f.Type == UserType.Qfau.ToString());
        if (messageType == MessageType.PutApplicationOnHold)
        {
            SetQfauReviewStatus(ApplicationStatus.OnHold, feedbacksToUpdate, qfauReview);
        } else if (messageType == MessageType.UnlockApplication) 
        {
            SetQfauReviewStatus(ApplicationStatus.Draft, feedbacksToUpdate, qfauReview);
        }

        await _feedbackRepository.UpdateAsync(feedbacksToUpdate);
    }

    private static void SetQfauReviewStatus(ApplicationStatus status, List<ApplicationReviewFeedback> feedbacksToUpdate, ApplicationReviewFeedback? qfauReview)
    {
        if (qfauReview != null)
        {
            qfauReview.Status = status.ToString();

            if (!feedbacksToUpdate.Any(f => f.Type == UserType.Qfau.ToString()))
            {
                feedbacksToUpdate.Add(qfauReview);
            }
        }
    }

    private static void HandleActionMessages(Data.Entities.Application.Application application, MessageType messageType)
    {
        if (messageType == MessageType.UnlockApplication)
        {
            application.Status = ApplicationStatus.Draft.ToString();
            application.Submitted = false;
            application.SubmittedAt = null;
        }
        else if (messageType == MessageType.PutApplicationOnHold)
        {
            application.Status = ApplicationStatus.OnHold.ToString();
        }
    }
}
