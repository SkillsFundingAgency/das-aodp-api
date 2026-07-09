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

        if (lastDateForRegistration > publicationDate)
        {
            var currentAcademicYear = _academicYearProvider.GetCurrentAcademicYearEndDate();
            var academicYearForLastDateForRegistration = _academicYearProvider.GetAcademicYearEndForDate(lastDateForRegistration);

            var ilrFinalSubmissionDeadline = _ilrSubmissionDeadlinesProvider.GetFinalSubmissionDeadline();

            if (academicYearForLastDateForRegistration > currentAcademicYear)
            {
                fundingApprovalEndDate = publicationDate >= ilrFinalSubmissionDeadline.Date ? currentAcademicYear.AddYears(2) : currentAcademicYear.AddYears(1);
            }
            else
            {
                fundingApprovalEndDate = currentAcademicYear;
            }

            if (pldnsDate is not null && _academicYearProvider.AreDatesWithinSameAcademicYear(pldnsDate, fundingApprovalEndDate))
            {
                fundingApprovalEndDate = DateOnly.FromDateTime(pldnsDate!.Value);
            }

            return fundingApprovalEndDate;
        }

        // Captures when the last date for registration is before the publication date but the last date for registration is after the funding approval end date for the funding stream or if the funding approval end date for the funding stream is null
        // then it means we can set the funding approval end date to the publication date as theres more time to fund it between the last date of registration and the publication date.
        if (lastDateForRegistration > qaaQualification.GetFundingApprovalEndDateForFundingStream(fundingStream) ||
            qaaQualification.GetFundingApprovalEndDateForFundingStream(fundingStream) is null)
        {
            fundingApprovalEndDate = publicationDate;
        }

        return fundingApprovalEndDate;
    }
}