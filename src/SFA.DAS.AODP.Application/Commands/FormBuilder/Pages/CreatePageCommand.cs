using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class CreatePageCommand : IRequest<BaseMediatrResponse<CreatePageCommandResponse>>
{
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; }
}
