using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class CreatePageCommandRequestHandler : IRequestHandler<CreatePageCommandRequest, CreatePageCommandResponse>
{

    public CreatePageCommandRequestHandler()
    {
    }

    public async Task<CreatePageCommandResponse> Handle(CreatePageCommandRequest request, CancellationToken cancellationToken)
    {
        var response = new CreatePageCommandResponse();
        try
        {
            //var page = new Page
            //{
            //    SectionId = request.SectionId,
            //    Title = request.Title,
            //    Description = request.Description,
            //    Order = request.Order,
            //    NextPageId = request.NextPageId
            //};

            // _pageRepository.Add(page);

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
