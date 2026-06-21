using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
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
    public async Task<DateOnly?> CalculateFundingApprovalEndDateAsync(RegulatedQaaQualification qaaQualification, FundingStream fundingStream, DateOnly publicationDate, CancellationToken cancellationToken)
    {
        var fundingApprovalEndDate = qaaQualification.GetFundingApprovalEndDateForFundingStream(fundingStream);
        var lastDateForRegistration = qaaQualification.LastDateForRegistration;

        var pldns = await _pldnsRepository.GetPldnsByQanAsync(qaaQualification.AimCode, cancellationToken);
        if (pldns is not null)
        {
            var pldnsDates = new List<DateTime?>()
            {
                pldns.Loans, pldns.Pldns16To19, pldns.LegalEntitlementL2L3
            };

            var minPldnsDate = pldnsDates.Where(o => o is not null).Min();
            if (minPldnsDate is not null)
            {
                fundingApprovalEndDate = DateOnly.FromDateTime(minPldnsDate!.Value);
                return fundingApprovalEndDate;
            }
        } 
        
        if (lastDateForRegistration > publicationDate)
        {
            var currentAcademicYear = _academicYearProvider.GetCurrentAcademicYearEndDate();
            var academicYearForLastDateForRegistration = _academicYearProvider.GetAcademicYearEndForDate(lastDateForRegistration);

            var ilrFinalSubmissionDeadline = _ilrSubmissionDeadlinesProvider.GetFinalSubmissionDeadline();

            if (academicYearForLastDateForRegistration > currentAcademicYear)
            {
                fundingApprovalEndDate = _clockProvider.Today >= ilrFinalSubmissionDeadline.Date ? currentAcademicYear.AddYears(2) : currentAcademicYear.AddYears(1);
            }
            else
            {
                fundingApprovalEndDate = currentAcademicYear;
            }
            
            return fundingApprovalEndDate;
        }
        
        if (lastDateForRegistration < publicationDate)
        {
            if (lastDateForRegistration > qaaQualification.GetFundingApprovalEndDateForFundingStream(fundingStream) ||
                qaaQualification.GetFundingApprovalEndDateForFundingStream(fundingStream) is null)
            {
                fundingApprovalEndDate = publicationDate;
            }
        }

        return fundingApprovalEndDate;
    }
}