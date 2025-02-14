using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetNewQualificationsQueryHandler : IRequestHandler<GetNewQualificationsQuery, BaseMediatrResponse<GetNewQualificationsQueryResponse>>
    {
        private readonly INewQualificationsRepository _repository;

        public GetNewQualificationsQueryHandler(INewQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetNewQualificationsQueryResponse>> Handle(GetNewQualificationsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetNewQualificationsQueryResponse>();
            try
            {
                var qualifications = await _repository.GetAllNewQualificationsAsync();
                if (qualifications != null && qualifications.Any())
                {
                    response.Value = new GetNewQualificationsQueryResponse
                    {
                        NewQualifications = qualifications
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
                response.Success = false;
                response.ErrorMessage = "An error occurred while retrieving new qualifications.";
                response.InnerException = ex;
            }
            return response;
        }
    }
}


