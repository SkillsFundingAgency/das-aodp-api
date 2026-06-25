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
        var pldnsDate = pldns?.ForFundingStream(fundingStream);
        
        if (pldnsDate is not null && _academicYearProvider.IsWithinCurrentAcademicYear(pldnsDate))
        {
            fundingApprovalEndDate = DateOnly.FromDateTime(pldnsDate.Value);
            return fundingApprovalEndDate;
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

        if (lastDateForRegistration >= publicationDate)
        {
            return fundingApprovalEndDate;
        }

        if (lastDateForRegistration > qaaQualification.GetFundingApprovalEndDateForFundingStream(fundingStream) ||
            qaaQualification.GetFundingApprovalEndDateForFundingStream(fundingStream) is null)
        {
            fundingApprovalEndDate = publicationDate;
        }

        return fundingApprovalEndDate;
    }
}