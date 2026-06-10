using MediatR;
using SFA.DAS.AODP.Application.Queries.Application.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Infrastructure;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationExportDataQueryHandler :IRequestHandler<GetApplicationExportDataQuery, BaseMediatrResponse<GetApplicationExportDataQueryResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IApplicationRepository _applicationRepository;

        public GetApplicationExportDataQueryHandler(IMediator mediator, IApplicationRepository applicationRepository)
        {
            _mediator = mediator;
            _applicationRepository = applicationRepository;
        }

        public async Task<BaseMediatrResponse<GetApplicationExportDataQueryResponse>> Handle(GetApplicationExportDataQuery request, CancellationToken cancellationToken)
        {


            var response = new BaseMediatrResponse<GetApplicationExportDataQueryResponse>();
            response.Success = false;
            try
            {
                var applicationFormAnswers = await _mediator.Send(new GetApplicationFormAnswersByReviewIdQuery(request.ApplicationReviewId));

                var applicationId = applicationFormAnswers.Value.ApplicationId;

                var formPreview = await _mediator.Send(new GetApplicationFormPreviewByIdQuery(applicationId));

                var applicationSummary = await _applicationRepository.GetApplicationExportMetadataAsync(applicationId);

                response.Value = new GetApplicationExportDataQueryResponse
                {
                    ApplicationFormStructure = formPreview.Value,
                    ApplicationFormResponse = applicationFormAnswers.Value,
                    ApplicationMetadata = new ApplicationMetadataResponse(applicationSummary)
                };


                response.Success = true;
            }
            catch (Exception ex)
            {
                response.InnerException = ex;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
