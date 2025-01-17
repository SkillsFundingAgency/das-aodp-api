using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryRequestHandler : IRequestHandler<GetSectionByIdQueryRequest, GetSectionByIdQueryResponse>
{
    public GetSectionByIdQueryRequestHandler()
    {
    }

    public async Task<GetSectionByIdQueryResponse> Handle(GetSectionByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var response = new GetSectionByIdQueryResponse();
        response.Success = false;
        try
        {
            //response.Data = _sectionRepository.GetById(request.Id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
