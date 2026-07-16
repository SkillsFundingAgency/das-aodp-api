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
        var lastDateForRegistration = qaaQualification.LastDateForRegistration;
        var currentAcademicYearEndDate = _academicYearProvider.GetCurrentAcademicYearEndDate();
        var registrationAcademicYearEndDate = _academicYearProvider.GetAcademicYearEndForDate(lastDateForRegistration);
        var isRegistrationInFutureAcademicYear = registrationAcademicYearEndDate > currentAcademicYearEndDate;

        DateOnly fundingApprovalEndDate;

        if (isRegistrationInFutureAcademicYear)
        {
            var finalIlrSubmissionDeadline = _ilrSubmissionDeadlinesProvider.GetFinalSubmissionDeadline();
            var nextAcademicYearEndDate = currentAcademicYearEndDate.AddYears(1);
            var extendedAcademicYearEndDate = currentAcademicYearEndDate.AddYears(2);
            var shouldExtendForIlrDeadline = publicationDate > finalIlrSubmissionDeadline.Date
                && extendedAcademicYearEndDate <= registrationAcademicYearEndDate;

            fundingApprovalEndDate = shouldExtendForIlrDeadline
                ? extendedAcademicYearEndDate
                : nextAcademicYearEndDate;
        }
        else if (registrationAcademicYearEndDate == currentAcademicYearEndDate)
        {
            fundingApprovalEndDate = currentAcademicYearEndDate;
        }
        else
        {
            fundingApprovalEndDate = publicationDate;
        }

        var pldns = await _pldnsRepository.GetPldnsByQanAsync(qaaQualification.AimCode, cancellationToken);
        var pldnsDate = pldns?.ForFundingStream(fundingStream);

        if (pldnsDate is null)
        {
            return fundingApprovalEndDate;
        }

        var fundingStreamPldnsDate = DateOnly.FromDateTime(pldnsDate.Value);
        var shouldUsePldns = fundingStreamPldnsDate <= fundingApprovalEndDate
            || _academicYearProvider.AreDatesWithinSameAcademicYear(pldnsDate, fundingApprovalEndDate);

        return shouldUsePldns
            ? fundingStreamPldnsDate
            : fundingApprovalEndDate;
    }
}
