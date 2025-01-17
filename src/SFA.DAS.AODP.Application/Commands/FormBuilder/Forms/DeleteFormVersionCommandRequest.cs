using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormVersionCommandRequest : IRequest<DeleteFormVersionCommandResponse>
{
    public Guid Id { get; set; }
}
