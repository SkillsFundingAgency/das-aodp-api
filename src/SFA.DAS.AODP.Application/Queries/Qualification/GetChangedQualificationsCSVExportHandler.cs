using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetChangedQualificationsCSVExportHandler : IRequestHandler<GetChangedQualificationsCsvExportQuery, BaseMediatrResponse<GetChangedQualificationsCsvExportResponse>>
    {
        private readonly IChangedQualificationsRepository _repository;

        public GetChangedQualificationsCSVExportHandler(IChangedQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetChangedQualificationsCsvExportResponse>> Handle(GetChangedQualificationsCsvExportQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetChangedQualificationsCsvExportResponse>();

            try
            {
                var qualifications = await _repository.GetChangedQualificationsCSVExport();
                if (qualifications == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "No changed qualifications found.";
                }
                else if (qualifications.Any())
                {
                    response.Value = new GetChangedQualificationsCsvExportResponse
                    {
                        QualificationExports = qualifications
                    };
                    response.Success = true;
                }
                else
                {
                    response.Value = new GetChangedQualificationsCsvExportResponse
                    {
                        QualificationExports = new List<ChangedQualificationExport>()
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