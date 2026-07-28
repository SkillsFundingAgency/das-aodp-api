namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Provides a mechanism to determine what the ILR submission deadline is for a particular return period.
/// There are 14 return periods each year, with the year running from October to October. 
/// </summary>
public interface IIlrSubmissionDeadlinesProvider
{
    /// <summary>
    /// Gets the final ILR submission deadline for the current return period. The final submission deadline is the last date on which ILR data can be submitted for a particular return period, and is used as part of the funding approval end date calculation for Qaa qualifications to determine whether the academic year-end should be this academic year or pushed into the next as if the current date is past the final ILR return period submission deadline, then we move into the next academic year for the purposes of determining a funding approval date.
    /// </summary>
    /// <returns>A <see cref="IlrSubmissionDeadline"/> instance representing the final submission deadline.</returns>
    IlrSubmissionDeadline GetFinalSubmissionDeadline();
}