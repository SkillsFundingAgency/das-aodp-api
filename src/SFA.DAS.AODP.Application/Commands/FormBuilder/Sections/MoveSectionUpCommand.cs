using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class MoveSectionUpCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
}
