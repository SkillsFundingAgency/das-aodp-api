using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class UpdatePageCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; }
}