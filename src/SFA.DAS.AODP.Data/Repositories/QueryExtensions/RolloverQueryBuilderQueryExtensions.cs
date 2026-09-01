using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.ValueObjects;

namespace SFA.DAS.AODP.Data.Repositories.QueryExtensions;

/// <summary>
/// Provides extension methods for filtering <see cref="QualificationVersions"/> queries based on various criteria.
/// </summary>
public static class RolloverQueryBuilderQueryExtensions
{
    /// <summary>
    /// Adds a filter to the query to include only qualifications that match the specified level IDs.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="levelIds">The level IDs to match.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<RolloverCandidates> WithLevelFilter(
        this IQueryable<RolloverCandidates> query, IEnumerable<int> levelIds) 
        => query
            .Where(qv => levelIds.Select(o => QualificationLevel.FromId(o).ToString()).Contains(qv.QualificationVersion.Level));

    /// <summary>
    /// Adds a filter to the query to include only qualifications that match the specified type IDs.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="typeIds">The type IDs to match.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<RolloverCandidates> WithTypeFilter(
        this IQueryable<RolloverCandidates> query, IEnumerable<int> typeIds) 
        => query.Where(qv => typeIds.Select(o => QualificationType.FromId(o).ToString()).Contains(qv.QualificationVersion.Type));

    /// <summary>
    /// Adds a filter to the query to include only qualifications that match the specified sector subject area IDs.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="sectorSubjectAreaIds">The sector subject area IDs to match.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<RolloverCandidates> WithSectorSubjectAreaFilter(
        this IQueryable<RolloverCandidates> query, IEnumerable<string> sectorSubjectAreaIds) 
        => query.Where(qv => sectorSubjectAreaIds.Select(o => SectorSubjectArea.FromFullCode(o).ToString()).Contains(qv.QualificationVersion.Ssa));

    /// <summary>
    /// Adds a filter to the query to include only qualifications that match the specified awarding organisation IDs.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="awardingOrganisationIds">The awarding organisation IDs to match.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<RolloverCandidates> WithAwardingOrganisationFilter(
        this IQueryable<RolloverCandidates> query, IEnumerable<string> awardingOrganisationIds)
        => query
            .Where(o => awardingOrganisationIds.Contains(o.QualificationVersion.Organisation.RecognitionNumber));

    /// <summary>
    /// Applies all specified filters (level, type, sector subject area, and awarding organisation) to the query.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="levelIds">The level IDs to match.</param>
    /// <param name="typeIds">The type IDs to match.</param>
    /// <param name="sectorSubjectAreaIds">The sector subject area IDs to match.</param>
    /// <param name="awardingOrganisationIds">The awarding organisation IDs to match.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<RolloverCandidates> WithAllFilters(
        this IQueryable<RolloverCandidates> query,
        IEnumerable<int> levelIds, 
        IEnumerable<int> typeIds, 
        IEnumerable<string> sectorSubjectAreaIds, 
        IEnumerable<string> awardingOrganisationIds) 
        => query
            .WithLevelFilter(levelIds)
            .WithTypeFilter(typeIds)
            .WithSectorSubjectAreaFilter(sectorSubjectAreaIds)
            .WithAwardingOrganisationFilter(awardingOrganisationIds);
}