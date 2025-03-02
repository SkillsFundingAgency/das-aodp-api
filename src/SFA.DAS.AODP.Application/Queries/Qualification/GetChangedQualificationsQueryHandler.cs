using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetChangedQualificationsQueryHandler : IRequestHandler<GetChangedQualificationsQuery, BaseMediatrResponse<GetChangedQualificationsQueryResponse>>
    {
        private readonly IQualificationsRepository _repository;

        public GetChangedQualificationsQueryHandler(IQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetChangedQualificationsQueryResponse>> Handle(GetChangedQualificationsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetChangedQualificationsQueryResponse>();
            try
            {
                var qualifications = await _repository.GetAllChangedQualificationsAsync();
                if (qualifications != null && qualifications.Any())
                {
                    response.Value = new GetChangedQualificationsQueryResponse
                    {
                        ChangedQualifications = qualifications
                    };
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "No new qualifications found.";
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}


