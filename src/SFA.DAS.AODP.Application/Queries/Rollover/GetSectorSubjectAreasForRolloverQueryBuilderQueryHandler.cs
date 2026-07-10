using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetSectorSubjectAreasForRolloverQueryBuilderQueryHandler(IRolloverRepository repository)
    : IRequestHandler<GetSectorSubjectAreasForRolloverQueryBuilderQuery, BaseMediatrResponse<GetSectorSubjectAreasForRolloverQueryBuilderQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetSectorSubjectAreasForRolloverQueryBuilderQueryResponse>> Handle(
        GetSectorSubjectAreasForRolloverQueryBuilderQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetSectorSubjectAreasForRolloverQueryBuilderQueryResponse>();

        try
        {
            response.Value.SectorSubjectAreas = await repository.GetSectorSubjectAreasForRolloverQueryBuilderAsync(request.Filters, cancellationToken);
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
