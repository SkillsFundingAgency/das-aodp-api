using Azure;
using MediatR;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{

    public class GetActionTypes : IRequestHandler<GetActionTypesQuery, BaseMediatrResponse<GetActionTypesResponse>>
    {
        private readonly IChangedQualificationsRepository _repository;

        public GetActionTypes(IChangedQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetActionTypesResponse>> Handle(GetActionTypesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetActionTypesResponse>();
            try
            {
                var actionTypes = await _repository.GetActionTypes();
                ;
                if (actionTypes != null)
                {
                    response.Value = new GetActionTypesResponse()
                    {
                        ActionTypes = actionTypes
                    };

                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = $"No ActionTypes ";
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


