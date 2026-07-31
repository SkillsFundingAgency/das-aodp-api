using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetQualificationVersionsForRolloverQueryBuilderQueryHandler(IRolloverRepository repository)
    : IRequestHandler<GetQualificationVersionsForRolloverQueryBuilderQuery, BaseMediatrResponse<GetQualificationVersionsForRolloverQueryBuilderQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetQualificationVersionsForRolloverQueryBuilderQueryResponse>> Handle(
        GetQualificationVersionsForRolloverQueryBuilderQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQualificationVersionsForRolloverQueryBuilderQueryResponse>();

        try
        {
            response.Value.QualificationVersions =
                await repository.GetQualificationVersionsForRolloverQueryBuilderAsync(request.Filters, cancellationToken);
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
