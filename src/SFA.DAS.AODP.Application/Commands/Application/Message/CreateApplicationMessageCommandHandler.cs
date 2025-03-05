using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Models.Form;

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

            var messageId = await _messageRepository.CreateAsync(new()
            {
                ApplicationId = request.ApplicationId,
                Text = request.MessageText,
                Type = request.MessageType,
                SharedWithAwardingOrganisation = request.SharedWithAwardingOrganisation,
                SharedWithDfe = request.SharedWithDfe,
                SharedWithOfqual = request.SharedWithOfqual,
                SharedWithSkillsEngland = request.SharedWithSkillsEngland,
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
