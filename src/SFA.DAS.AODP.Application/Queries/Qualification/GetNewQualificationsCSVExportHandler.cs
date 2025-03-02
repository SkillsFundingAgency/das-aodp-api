using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetNewQualificationsCSVExportHandler : IRequestHandler<GetNewQualificationsCsvExportQuery, BaseMediatrResponse<GetNewQualificationsCsvExportResponse>>
    {
        private readonly IQualificationsRepository _repository;

        public GetNewQualificationsCSVExportHandler(IQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetNewQualificationsCsvExportResponse>> Handle(GetNewQualificationsCsvExportQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetNewQualificationsCsvExportResponse>();

            try
            {
                var qualifications = await _repository.GetNewQualificationsCSVExport();
                if (qualifications == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "No new qualifications found.";
                }
                else if (qualifications.Any())
                {
                    response.Value = new GetNewQualificationsCsvExportResponse
                    {
                        QualificationExports = qualifications
                    };
                    response.Success = true;
                }
                else
                {
                    response.Value = new GetNewQualificationsCsvExportResponse
                    {
                        QualificationExports = new List<QualificationExport>()
                    };
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}