using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class RolloverRepository : IRolloverRepository
{
    private readonly IApplicationDbContext _context;

    public RolloverRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken)
    {
        var dbSet = _context.RolloverWorkflowCandidates;

        var totalRecords = await dbSet.AsNoTracking().CountAsync(cancellationToken);

        return totalRecords;
    }

    public async Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowCandidates.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverWorkflowCandidatesP1Checks>> GetRolloverWorkflowCandidatesP1ChecksAsync(CancellationToken cancellationToken)
    {
        var dbSet = _context.RolloverWorkflowCandidatesP1Checks;
        var query = await dbSet.AsNoTracking().ToListAsync(cancellationToken);

        return query;
    }

    public async Task UpdateRolloverWorkflowCandidatesAsync(IEnumerable<RolloverWorkflowCandidate> candidates, CancellationToken cancellationToken)
    {
        var list = candidates as IList<RolloverWorkflowCandidate> ?? candidates.ToList();
        if (!list.Any())
            return;

        _context.RolloverWorkflowCandidates.UpdateRange(list);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(rc => new RolloverCandidate
            {
                Id = rc.Id,
                QualificationVersionId = rc.QualificationVersionId,
                FundingOfferId = rc.FundingOfferId,
                FundingOfferName = rc.FundingOffer != null ? rc.FundingOffer.Name : null,
                QualificationNumber = rc.QualificationVersion != null && rc.QualificationVersion.Qualification != null ?
                    rc.QualificationVersion.Qualification.Qan : null,
                AcademicYear = rc.AcademicYear
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderAwardingOrganisation>> GetAwardingOrganisationsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(
                _context.QualificationVersions.AsNoTracking(),
                filters,
                includeAwardingOrganisations: false)
            .Select(qv => qv.Organisation)
            .Distinct()
            .OrderBy(organisation => organisation.NameOfqual)
            .ThenBy(organisation => organisation.NameLegal)
            .Select(organisation => new RolloverQueryBuilderAwardingOrganisation
            {
                Id = organisation.Id,
                Ukprn = organisation.Ukprn,
                RecognitionNumber = organisation.RecognitionNumber,
                NameLegal = organisation.NameLegal,
                NameOfqual = organisation.NameOfqual,
                NameGovUk = organisation.NameGovUk,
                Name_Dsi = organisation.Name_Dsi,
                Acronym = organisation.Acronym
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQualificationVersion>> GetQualificationVersionsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(
                _context.QualificationVersions.AsNoTracking(),
                filters,
                includeAwardingOrganisations: true)
            .OrderBy(qv => qv.Qualification.Qan)
            .ThenBy(qv => qv.Name ?? qv.Qualification.QualificationName)
            .Select(qv => new RolloverQualificationVersion
            {
                Id = qv.Id,
                QualificationReference = qv.Qualification.Qan,
                QualificationName = qv.Name ?? qv.Qualification.QualificationName,
                AwardingOrganisationId = qv.AwardingOrganisationId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(rc =>
                rolloverCandidateIds.Contains(rc.Id) && rc.IsActive)
            .Select(rc => new RolloverCandidate
            {
                Id = rc.Id,
                QualificationVersionId = rc.QualificationVersionId,
                FundingOfferId = rc.FundingOfferId,
                RolloverRound = rc.RolloverRound,
                AcademicYear = rc.AcademicYear,
                PreviousFundingEndDate = rc.PreviousFundingEndDate,
                NewFundingEndDate = rc.NewFundingEndDate
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> CreateRolloverWorkflowRunAsync(RolloverWorkflowRun workflowRun, CancellationToken cancellationToken = default)
    {
        _context.RolloverWorkflowRuns.Add(workflowRun);
        await _context.SaveChangesAsync(cancellationToken);
        return workflowRun.Id;
    }

    public async Task CreateRolloverWorkflowCandidatesAsync(
        IEnumerable<RolloverWorkflowCandidate> workflowCandidates,
        CancellationToken cancellationToken)
    {
        var incomingRolloverCandidates = workflowCandidates.ToList();

        var incomingCandidateIds = incomingRolloverCandidates
            .Select(x => x.RolloverCandidatesId)
            .ToList();

        var existingWorkflowCandidates = await _context.RolloverWorkflowCandidates
            .Where(x => incomingCandidateIds.Contains(x.RolloverCandidatesId))
            .ToListAsync(cancellationToken);

        _context.RolloverWorkflowCandidates.RemoveRange(existingWorkflowCandidates);

        _context.RolloverWorkflowCandidates.AddRange(incomingRolloverCandidates);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRolloverWorkflowRunFundingOffersAsync(IEnumerable<RolloverWorkflowRunFundingOffer> workflowFundingOffers, CancellationToken cancellationToken)
    {
        _context.RolloverWorkflowRunFundingOffers.AddRange(workflowFundingOffers);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<QualificationVersions> ApplyRolloverQueryBuilderFilters(
        IQueryable<QualificationVersions> query,
        RolloverQueryBuilderRequest filters,
        bool includeAwardingOrganisations)
    {
        var allQualificationVersions = _context.QualificationVersions.AsNoTracking();

        query = query
            .Where(qv => qv.EligibleForFunding == true)
            .Where(qv => !allQualificationVersions.Any(otherVersion =>
                otherVersion.QualificationId == qv.QualificationId &&
                (
                    (otherVersion.Version ?? 0) > (qv.Version ?? 0) ||
                    (
                        (otherVersion.Version ?? 0) == (qv.Version ?? 0) &&
                        otherVersion.LastUpdatedDate > qv.LastUpdatedDate
                    ) ||
                    (
                        (otherVersion.Version ?? 0) == (qv.Version ?? 0) &&
                        otherVersion.LastUpdatedDate == qv.LastUpdatedDate &&
                        otherVersion.InsertedDate > qv.InsertedDate
                    )
                )));

        if (filters.LevelIds.Count > 0)
        {
            query = query.Where(qv => qv.LevelId.HasValue && filters.LevelIds.Contains(qv.LevelId.Value));
        }

        if (filters.TypeIds.Count > 0)
        {
            query = query.Where(qv => qv.TypeId.HasValue && filters.TypeIds.Contains(qv.TypeId.Value));
        }

        if (filters.SectorSubjectAreaIds.Count > 0)
        {
            query = query.Where(qv => filters.SectorSubjectAreaIds.Contains(qv.Ssa));
        }

        if (includeAwardingOrganisations && filters.AwardingOrganisationIds.Count > 0)
        {
            query = query.Where(qv => filters.AwardingOrganisationIds.Contains(qv.AwardingOrganisationId));
        }

        return query;
    }

}
