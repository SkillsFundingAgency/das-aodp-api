using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class DeletePageCommandRequestHandler : IRequestHandler<DeletePageCommandRequest, DeletePageCommandResponse>
{
    public DeletePageCommandRequestHandler()
    {
    }

    public async Task<DeletePageCommandResponse> Handle(DeletePageCommandRequest request, CancellationToken cancellationToken)
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
