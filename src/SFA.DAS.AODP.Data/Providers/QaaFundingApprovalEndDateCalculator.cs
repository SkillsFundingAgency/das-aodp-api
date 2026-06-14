using SFA.DAS.AODP.Data.Repositories.Pldns;

namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Default implementation for <see cref="IQaaFundingApprovalEndDateCalculator"/>.
/// </summary>
public class QaaFundingApprovalEndDateCalculator(
    ISystemClockProvider clockProvider,
    IIlrSubmissionDeadlinesProvider ilrSubmissionDeadlinesProvider,
    IAcademicYearProvider academicYearProvider, 
    IPldnsRepository pldnsRepository) : IQaaFundingApprovalEndDateCalculator
{
    private readonly ISystemClockProvider _clockProvider = clockProvider;
    private readonly IIlrSubmissionDeadlinesProvider _ilrSubmissionDeadlinesProvider = ilrSubmissionDeadlinesProvider;
    private readonly IAcademicYearProvider _academicYearProvider = academicYearProvider;
    private readonly IPldnsRepository _pldnsRepository = pldnsRepository;

    /// <inheritdoc/>.
    public async Task<DateOnly?> CalculateFundingApprovalEndDateAsync(string qan, DateOnly lastDateForRegistration, DateOnly? currentFundingApprovalEndDate, DateOnly publicationDate, CancellationToken cancellationToken)
    {
        var fundingApprovalEndDate = currentFundingApprovalEndDate;

        var pldns = await _pldnsRepository.GetPldnsByQanAsync(qan, cancellationToken);
        if (pldns is not null)
        {
            var pldnsDates = new List<DateTime?>()
            {
                pldns.Loans, pldns.Pldns16To19, pldns.LegalEntitlementL2L3
            };

            var minPldnsDate = pldnsDates.Where(o => o is not null).Min();
            if (minPldnsDate is not null && _academicYearProvider.IsWithinCurrentAcademicYear(minPldnsDate))
            {
                fundingApprovalEndDate = DateOnly.FromDateTime(minPldnsDate!.Value);
                return fundingApprovalEndDate;
            }
        } 
        
        if (lastDateForRegistration > publicationDate)
        {
            var currentAcademicYear = _academicYearProvider.GetCurrentAcademicYearEndDate();
            var ilrFinalSubmissionDeadline = _ilrSubmissionDeadlinesProvider.GetFinalSubmissionDeadline();

            if (_clockProvider.Today >= ilrFinalSubmissionDeadline.Date)
            {
                currentAcademicYear = currentAcademicYear.AddYears(1);
            }

            var dates = new List<DateOnly>
            {
                lastDateForRegistration,
                currentAcademicYear
            };

            fundingApprovalEndDate = dates.Min();
            return fundingApprovalEndDate;
        }
        
        if (lastDateForRegistration < publicationDate)
        {
            if (lastDateForRegistration > currentFundingApprovalEndDate ||
                currentFundingApprovalEndDate is null)
            {
                fundingApprovalEndDate = lastDateForRegistration;
            }
        }

        return fundingApprovalEndDate;
    }
}