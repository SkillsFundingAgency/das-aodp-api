using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormCommandHandler : IRequestHandler<UpdateFormCommand, UpdateFormCommandResponse>
{

    public UpdateFormCommandHandler()
    {
    }

    public async Task<UpdateFormCommandResponse> Handle(UpdateFormCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateFormCommandResponse();
        response.Success = false;


        return response;
    }
}
