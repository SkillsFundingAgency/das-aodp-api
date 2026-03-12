using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetRolloverWorkflowCandidatesCountQueryHandler : IRequestHandler<GetRolloverWorkflowCandidatesCountQuery, BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>
{
    private readonly IRolloverRepository _repository;

    public GetRolloverWorkflowCandidatesCountQueryHandler(IRolloverRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>> Handle(GetRolloverWorkflowCandidatesCountQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>();
        try
        {
            var result = await _repository.GetRolloverWorkflowCandidatesCountAsync(cancellationToken);

            if (result != null)
            {
                response.Value = new GetRolloverWorkflowCandidatesCountQueryResponse
                {
                    TotalRecords = result.TotalRecords
                };
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = "No rollover workflow candidates found.";
            }
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