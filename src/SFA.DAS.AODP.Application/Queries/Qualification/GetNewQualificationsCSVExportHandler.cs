using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetNewQualificationsCSVExportHandler : IRequestHandler<GetNewQualificationsCSVExportQuery, BaseMediatrResponse<GetNewQualificationsCSVExportResponse>>
    {
        private readonly INewQualificationsRepository _repository;

        public GetNewQualificationsCSVExportHandler(INewQualificationsRepository repository)
        {
            _repository = repository;
        }
        public async Task<BaseMediatrResponse<GetNewQualificationsCSVExportResponse>> Handle(GetNewQualificationsCSVExportQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetNewQualificationsCSVExportResponse>();
            
            try
            {
                var qualifications = await _repository.GetNewQualificationsCSVExport();
                if (qualifications != null && qualifications.Any())
                {
                    response.Value = new GetNewQualificationsCSVExportResponse
                    {
                        QualificationExports = qualifications
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
