using MediatR;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetRolloverStartSummaryQueryHandler(IRolloverRepository repository, IAcademicYearService academicYearService)
    : IRequestHandler<GetRolloverStartSummaryQuery, BaseMediatrResponse<GetRolloverStartSummaryQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetRolloverStartSummaryQueryResponse>> Handle(
        GetRolloverStartSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetRolloverStartSummaryQueryResponse>();

        try
        {
            var currentAcademicYear = academicYearService.GetCurrentAcademicYear();
            
            var result = await repository.GetRolloverStartSummaryAsync(currentAcademicYear, cancellationToken);

            if (result != null) 
            {
                response.Value = new GetRolloverStartSummaryQueryResponse
                {
                    TotalCandidatesCount = result.TotalCandidatesCount,
                    CandidatesEligibleCount = result.CandidatesEligibleCount,
                    CandidatesIneligibleCount = result.CandidatesIneligibleCount,
                    CandidatesRemainingCount = result.CandidatesRemainingCount
                };

                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = "Not sure what goes here.";
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
