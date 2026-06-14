namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Provides an interface for calculating the funding approval end date for a Qaa qualification.
/// </summary>
public interface IQaaFundingApprovalEndDateCalculator
{
    /// <summary>
    /// Calculates the funding approval end date for a Qaa qualification based on the last date for registration, the current funding approval end date, the publication date of the output file and any
    /// </summary>
    /// <param name="qan">The qan or otherwise known as the AIM Code for the Qaa qualification.</param>
    /// <param name="lastDateForRegistration">The last date for registration, used to determine whether this date should be used or another earlier date such as the academic year-end.</param>
    /// <param name="currentFundingApprovalEndDate">The current funding approval end date.</param>
    /// <param name="publicationDate">The date on which the output file will be published.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    /// <returns>The calculated funding approval end date.</returns>
    Task<DateOnly?> CalculateFundingApprovalEndDateAsync(string qan, DateOnly lastDateForRegistration, DateOnly? currentFundingApprovalEndDate, DateOnly publicationDate, CancellationToken cancellationToken);
}