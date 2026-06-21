using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Provides an interface for calculating the funding approval end date for a Qaa qualification.
/// </summary>
public interface IQaaFundingApprovalEndDateCalculator
{
    /// <summary>
    /// Calculates the funding approval end date for a Qaa qualification based on the last date for registration, the current funding approval end date, the publication date of the output file and any
    /// </summary>
    /// <param name="qaaQualification">The qaa qualification object.</param>
    /// <param name="fundingStream">The funding stream that the funding approval end date is being calculated for.</param>
    /// <param name="publicationDate">The date on which the output file with the Qaa data will next be published, used as part of the calculations for the funding approval end dates.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    /// <returns>The calculated funding approval end date.</returns>
    Task<DateOnly?> CalculateFundingApprovalEndDateAsync(RegulatedQaaQualification qaaQualification, FundingStream fundingStream, DateOnly publicationDate, CancellationToken cancellationToken);
}