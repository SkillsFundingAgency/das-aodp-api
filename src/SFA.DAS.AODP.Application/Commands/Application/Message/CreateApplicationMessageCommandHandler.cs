using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Message;

public class CreateApplicationMessageCommandHandler : IRequestHandler<CreateApplicationMessageCommand, BaseMediatrResponse<CreateApplicationMessageCommandResponse>>
{
    private readonly IApplicationMessagesRepository _messageRepository;

    public CreateApplicationMessageCommandHandler(IApplicationMessagesRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<BaseMediatrResponse<CreateApplicationMessageCommandResponse>> Handle(CreateApplicationMessageCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<CreateApplicationMessageCommandResponse>();

        try
        {
            // any check on the application status here?

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
}
