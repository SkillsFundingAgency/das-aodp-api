using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetLevelsForRolloverQueryBuilderQueryHandler(IRolloverRepository repository)
    : IRequestHandler<GetLevelsForRolloverQueryBuilderQuery, BaseMediatrResponse<GetLevelsForRolloverQueryBuilderQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetLevelsForRolloverQueryBuilderQueryResponse>> Handle(
        GetLevelsForRolloverQueryBuilderQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetLevelsForRolloverQueryBuilderQueryResponse>();

        try
        {
            response.Value.Levels = await repository.GetAllLevelsForRolloverQueryBuilderAsync(cancellationToken);
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
