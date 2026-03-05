using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetRolloverWorkflowCandidatesQueryHandler : IRequestHandler<GetRolloverWorkflowCandidatesQuery, BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>>
{
    private readonly IRolloverRepository _repository;

    public GetRolloverWorkflowCandidatesQueryHandler(IRolloverRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>> Handle(GetRolloverWorkflowCandidatesQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>();
        try
        {
            var result = await _repository.GetAllRolloverWorkflowCandidatesAsync(request.Skip, request.Take);

            if (result != null)
            {
                response.Value = new GetRolloverWorkflowCandidatesQueryResponse
                {
                    Data = result.Data,
                    Skip = result.Skip,
                    Take = result.Take,
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