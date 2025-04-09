using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

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
                var result = await _repository.GetAllNewQualificationsAsync(
                    request.Skip, 
                    request.Take, 
                    new NewQualificationsFilter() 
                    {
                        Name = request.Name,
                        Organisation = request.Organisation,
                        QAN = request.QAN, 
                        ProcessStatusIds = request.ProcessStatusIds,
                    });
                if (result != null)
                {
                    response.Value = new GetNewQualificationsQueryResponse
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


