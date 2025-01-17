using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class DeletePageCommandHandler : IRequestHandler<DeletePageCommand, DeletePageCommandResponse>
{
    public DeletePageCommandHandler()
    {
    }

    public async Task<DeletePageCommandResponse> Handle(DeletePageCommand request, CancellationToken cancellationToken)
    {
        var response = new DeletePageCommandResponse();

        try
        {
            //var page = _pageRepository.GetById(request.Id);
            //if (page == null)
            //{
            //    response.Success = false;
            //    response.ErrorMessage = $"Page with id '{page!.Id}' could not be found.";

            //    return response;
            //}

            //_pageRepository.Delete(request.Id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
