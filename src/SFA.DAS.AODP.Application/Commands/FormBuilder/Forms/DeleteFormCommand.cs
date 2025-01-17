using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormCommand : IRequest<DeleteFormCommandResponse>
{
    public Guid Id { get; set; }
}
