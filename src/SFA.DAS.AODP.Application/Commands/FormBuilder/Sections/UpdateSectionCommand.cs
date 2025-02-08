using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class UpdateSectionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid FormVersionId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
}
