using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class DeleteSectionCommandRequest : IRequest<DeleteSectionCommandResponse>
{
    public Guid Id { get; set; }
}
