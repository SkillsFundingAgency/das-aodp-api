using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverCandidatesQueryHandler : IRequestHandler<GetRolloverCandidatesQuery, BaseMediatrResponse<GetRolloverCandidatesQueryResponse>>
    {
        private readonly IRolloverRepository _repository;

        public GetRolloverCandidatesQueryHandler(IRolloverRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetRolloverCandidatesQueryResponse>> Handle(GetRolloverCandidatesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetRolloverCandidatesQueryResponse>();

            try
            {
                var result = await _repository.GetRolloverCandidatesAsync();

                if (result != null)
                {
                    response.Value = new GetRolloverCandidatesQueryResponse
                    {
                        RolloverCandidates = result
                    };

                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "No rollover candidates found.";
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
}
