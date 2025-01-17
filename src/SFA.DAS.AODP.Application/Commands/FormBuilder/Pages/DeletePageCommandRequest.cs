using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class DeletePageCommandRequest : IRequest<DeletePageCommandResponse>
{
    public Guid Id { get; set; }
}
