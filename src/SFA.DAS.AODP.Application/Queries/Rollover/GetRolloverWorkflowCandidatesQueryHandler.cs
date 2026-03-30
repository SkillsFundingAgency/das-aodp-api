using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverWorkflowCandidatesQueryHandler : IRequestHandler<GetRolloverWorkflowCandidatesQuery, BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>>
    {
        private readonly IRolloverRepository _repository;

        public GetRolloverWorkflowCandidatesQueryHandler(IRolloverRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>> Handle(GetRolloverWorkflowCandidatesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>();

            try
            {
                var result = await _repository.GetAllRolloverWorkflowCandidatesAsync(cancellationToken);

                if (result != null)
                {
                    var workflowCandidates = result.Select(x=> new RolloverWorkflowCandidate 
                    {
                        Id = x.Id,
                        RolloverCandidatesId = x.RolloverCandidatesId,
                        RolloverWorkflowRunId = x.RolloverWorkflowRunId,
                        QualificationVersionId = x.QualificationVersionId,
                        FundingOfferId = x.FundingOfferId,
                        AcademicYear = x.AcademicYear,
                        PassP1 = x.PassP1,
                        P1FailureReason = x.P1FailureReason,
                        IncludedInP1Export = x.IncludedInP1Export,
                        IncludedInFinalUpload = x.IncludedInFinalUpload,
                        CurrentFundingEndDate = x.CurrentFundingEndDate,
                        ProposedFundingEndDate = x.ProposedFundingEndDate
                    }).ToList();

                    var workflowRunId = workflowCandidates
                        .Select(x => x.RolloverWorkflowRunId)
                        .Distinct()
                        .FirstOrDefault();

                    var workflowRunResult = await _repository.GeRolloverWorkflowRunByIdAsync(workflowRunId, cancellationToken);

                    response.Value = new GetRolloverWorkflowCandidatesQueryResponse
                    {
                        RolloverWorkflowCandidates = workflowCandidates,
                        WorkflowRunId = workflowRunId,
                        FundingEndDateEligibilityThreshold = workflowRunResult.OperationalEndDateEligibilityThreshold,
                        OperationalEndDateEligibilityThreshold = workflowRunResult.OperationalEndDateEligibilityThreshold,
                        MaximumApprovalFundingEndDate = workflowRunResult.OperationalEndDateEligibilityThreshold
                    };

                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "No rollover workflow candidates found.";
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
