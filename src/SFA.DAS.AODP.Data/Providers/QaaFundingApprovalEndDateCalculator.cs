using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Pldns;
using System.Security.Cryptography;

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

        // Rule 1: If the OED is in a future academic year (e.g. 26/27 or later) approve to end of the next academic year i.e. current + 1 year (FY1)
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
        // Rule 2: If the OED is in current academic year (e.g.) 25/26, approve to end of the current academic year (FY0)
        else if (registrationAcademicYearEndDate == currentAcademicYearEndDate)
        {
            fundingApprovalEndDate = currentAcademicYearEndDate;
        }
        // Rule 3: If the OED is in a previous academic year (FY1), hard stop at date of website publication
        else
        {
            fundingApprovalEndDate = publicationDate;
        }

        // Cross apply the PLDNS rule to ensure that the provisional date we have calculated doesn't exceed the PLDNS date for the funding stream.
        var pldns = await _pldnsRepository.GetPldnsByQanAsync(qaaQualification.AimCode, cancellationToken);
        var pldnsDate = pldns?.ForFundingStream(fundingStream);

        if (pldnsDate is null)
        {
            return fundingApprovalEndDate;
        }

        // If we have found a PLDNS date that is earlier than the funding approval end date, we should use that instead as it acts as a hard stop for funding.
        var fundingStreamPldnsDate = DateOnly.FromDateTime(pldnsDate.Value);
        var shouldUsePldns = fundingStreamPldnsDate <= fundingApprovalEndDate;

        return shouldUsePldns
            ? fundingStreamPldnsDate
            : fundingApprovalEndDate;
    }
}
