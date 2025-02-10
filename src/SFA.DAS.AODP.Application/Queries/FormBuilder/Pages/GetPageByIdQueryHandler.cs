using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetPageByIdQueryHandler(IPageRepository _pageRepository, IFormVersionRepository _formVersionRepository) : IRequestHandler<GetPageByIdQuery, BaseMediatrResponse<GetPageByIdQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetPageByIdQueryResponse>> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetPageByIdQueryResponse>();
        try
        {
            var page = await _pageRepository.GetPageByIdAsync(request.PageId);

            response.Value = page;

            response.Value.Editable = await _formVersionRepository.IsFormVersionEditable(request.FormVersionId);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
