using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class MovePageDownCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid PageId { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
}
