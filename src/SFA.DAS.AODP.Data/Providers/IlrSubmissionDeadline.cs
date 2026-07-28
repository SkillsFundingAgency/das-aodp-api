using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Value object representing the ILR submission deadline for a particular return period, containing the return period and the date of the final submission deadline for that return period.
/// </summary>
/// <param name="Period">The name of the return period.</param>
/// <param name="Date">The submission deadline date for this return period.</param>
[ExcludeFromCodeCoverage]
public sealed record IlrSubmissionDeadline(string Period, DateOnly Date);