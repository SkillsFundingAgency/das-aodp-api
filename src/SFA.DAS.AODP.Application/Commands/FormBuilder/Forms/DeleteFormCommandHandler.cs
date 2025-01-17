using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormCommandHandler : IRequestHandler<DeleteFormCommand, DeleteFormCommandResponse>
{

    public DeleteFormCommandHandler()
    {
    }

    public async Task<DeleteFormCommandResponse> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteFormCommandResponse();
        try
        {
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}