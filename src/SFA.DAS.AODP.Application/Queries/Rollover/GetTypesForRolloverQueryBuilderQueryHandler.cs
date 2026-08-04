using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetTypesForRolloverQueryBuilderQueryHandler(IRolloverRepository repository)
    : IRequestHandler<GetTypesForRolloverQueryBuilderQuery, BaseMediatrResponse<GetTypesForRolloverQueryBuilderQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetTypesForRolloverQueryBuilderQueryResponse>> Handle(
        GetTypesForRolloverQueryBuilderQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetTypesForRolloverQueryBuilderQueryResponse>();

        try
        {
            response.Value.Types = await repository.GetTypesForRolloverQueryBuilderAsync(request.Filters, cancellationToken);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }

        return response;
    }
}
