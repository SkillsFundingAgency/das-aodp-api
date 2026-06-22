using MediatR;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Infrastructure.Extensions;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverCandidatesForExportQueryHandler : IRequestHandler<GetRolloverCandidatesForExportQuery, BaseMediatrResponse<GetRolloverCandidatesForExportQueryResponse>>
    {
        private readonly IRolloverRepository _repository;
        private readonly IFundingExtensionCandidatesCsvBuilder _csvBuilder;


        public GetRolloverCandidatesForExportQueryHandler(IRolloverRepository repository, IFundingExtensionCandidatesCsvBuilder csvBuilder)
        {
            _repository = repository;
            _csvBuilder = csvBuilder;
        }

        public async Task<BaseMediatrResponse<GetRolloverCandidatesForExportQueryResponse>> Handle(GetRolloverCandidatesForExportQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetRolloverCandidatesForExportQueryResponse>();

            try
            {
                var result = await _repository.GetRolloverWorkflowCandidatesByRunId(request.RolloverWorkflowRunId, cancellationToken);

                var csvContent = _csvBuilder.Build(result);

                var currentDateString = DateOnly.FromDateTime(DateTime.UtcNow).ToFilenameDateFormat();

                response.Value = new GetRolloverCandidatesForExportQueryResponse
                {
                    FileContent = csvContent,
                    FileName = $"RolloverCandidates_SystemDraft_{currentDateString}.csv",
                    ContentType = "text/csv"
                };
                response.Success = true;

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
