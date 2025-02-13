using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class DeletePageCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public readonly Guid PageId;

    public DeletePageCommand(Guid pageId)
    {
        PageId = pageId;
    }
}