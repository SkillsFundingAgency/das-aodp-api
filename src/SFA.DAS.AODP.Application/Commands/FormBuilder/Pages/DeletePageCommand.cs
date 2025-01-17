using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class DeletePageCommand : IRequest<DeletePageCommandResponse>
{
    public Guid Id { get; set; }
}
