using MediatR;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Commands.Rollover;

public class UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler : IRequestHandler<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IRolloverRepository _rolloverRepository;

    public UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler(IRolloverRepository rolloverRepository)
    {
        _rolloverRepository = rolloverRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();
        try
        {
            var query = await _rolloverRepository.GetRolloverWorkflowCandidatesP1ChecksAsync(cancellationToken);

            var rolloverWorkflowCandidates = await _rolloverRepository.GetAllRolloverWorkflowCandidatesAsync(cancellationToken);
            var candidatesToUpdate = ProcessP1Checks(query, rolloverWorkflowCandidates);

            if (candidatesToUpdate.Count > 0)
            {
                await _rolloverRepository.UpdateRolloverWorkflowCandidatesAsync(candidatesToUpdate, cancellationToken);
            }
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

    private static List<RolloverWorkflowCandidate> ProcessP1Checks(IEnumerable<RolloverWorkflowCandidatesP1Checks> query, IEnumerable<RolloverWorkflowCandidate> rolloverWorkflowCandidates)
    {
        var candidatesToUpdate = new List<RolloverWorkflowCandidate>();

        foreach (var v in query)
        {
            // Find the persisted workflow candidate to update
            var candidate = rolloverWorkflowCandidates.FirstOrDefault(rwc => rwc.Id == v.WorkflowCandidateId);
            if (candidate == null)
                continue;

            var failures = ValidateP1Checks(v);

            var passP1 = failures.Count == 0;
            candidate.SetP1Result(passP1, passP1 ? null : string.Join("; ", failures));

            candidatesToUpdate.Add(candidate);
        }

        return candidatesToUpdate;
    }

    private static List<string> ValidateP1Checks(RolloverWorkflowCandidatesP1Checks checks)
    {
        var failures = new List<string>();

        // 1) Is the Funding Stream included in the RollOver
        if (checks.FundingStream == null)
            failures.Add("Funding Stream out of scope for RollOver");

        // 2) Latest Funding Approval End Date >= Threshold Date
        if (checks.LatestFundingApprovalEndDate.HasValue && checks.LatestFundingApprovalEndDate.Value < checks.ThresholdDate)
            failures.Add("Funding Approval End Date is before the Threshold");

        // 3) Operating End Date > Threshold Date  (If Operating End Date = Null, this should Pass the check)
        if (checks.OperationalEndDate.HasValue && checks.OperationalEndDate.Value <= checks.ThresholdDate)
            failures.Add("Operating End Date is before the Threshold");

        // 4) Offered in England = TRUE
        if (!checks.OfferedInEngland)
            failures.Add("Not Funded in England");

        // 6) GLH <= TQT
        if (checks.Glh > checks.Tqt)
            failures.Add("GLH > TQT");

        // 7) Does the Qualification appear in the Defunding (Defunded) List
        if (checks.IsOnDefundingList)
            failures.Add("Qualification is on Defunding (Defunded) List");

        return failures;
    }
}
