﻿using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetNewQualificationsCSVExportHandler : IRequestHandler<GetNewQualificationsCsvExportQuery, BaseMediatrResponse<GetNewQualificationsCsvExportResponse>>
    {
        private readonly INewQualificationsRepository _repository;

        public GetNewQualificationsCSVExportHandler(INewQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetNewQualificationsCsvExportResponse>> Handle(GetNewQualificationsCsvExportQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetNewQualificationsCsvExportResponse>();

            try
            {
                var qualifications = await _repository.GetNewQualificationsExport();
                if (qualifications == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "No new qualifications found.";
                }
                else
                {
                    response.Value = new GetNewQualificationsCsvExportResponse
                    {
                        QualificationExports = qualifications
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