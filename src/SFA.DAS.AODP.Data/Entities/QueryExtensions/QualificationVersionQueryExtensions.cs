using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Entities.QueryExtensions;

/// <summary>
/// Extension methods for querying QualificationVersions with specific filters and conditions.
/// </summary>
public static class QualificationVersionQueryExtensions
{
    /// <summary>
    /// Filters the query to include only QualificationVersions that are eligible for funding.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<QualificationVersions> WhereEligibleForFunding(this IQueryable<QualificationVersions> query) 
        => query.Where(qualificationVersion => qualificationVersion.EligibleForFunding == true);

    /// <summary>
    /// Filters the query to include only the latest version of each qualification based on version number.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<QualificationVersions> WhereLatestVersionPerQualification(
        this IQueryable<QualificationVersions> query)
    {
        var latestVersions =
            query
                .GroupBy(x => x.QualificationId)
                .Select(g => new
                {
                    QualificationId = g.Key,
                    Version = g.Max(x => x.Version)
                });

        return
            from qualificationVersion in query
            join latestVersion in latestVersions
                on new
                {
                    qualificationVersion.QualificationId,
                    qualificationVersion.Version
                }
                equals new
                {
                    latestVersion.QualificationId,
                    latestVersion.Version
                }
            select qualificationVersion;
    }
}