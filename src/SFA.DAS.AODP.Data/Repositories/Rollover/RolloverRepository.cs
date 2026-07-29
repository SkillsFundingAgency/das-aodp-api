using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.ValueObjects;
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

        var totalRecords = await dbSet
            .AsNoTracking()
            .CountAsync(x => !x.InvalidatedAt.HasValue, cancellationToken);

        return totalRecords;
    }

    public async Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowCandidates
            .Where(x => !x.InvalidatedAt.HasValue)
            .ToListAsync(cancellationToken);
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

    public async Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .WithAllSourceQualifications(_context)
            .Select(x => new RolloverCandidateDto
            {
                Id = x.CandidateId,
                SourceType = x.SourceType,
                SourceQualificationId = x.SourceQualificationId,
                FundingOfferId = x.FundingOfferId,
                FundingOfferName = x.FundingStreamName,
                QualificationNumber = x.QualificationReference,
                AcademicYear = x.AcademicYear
            })
            .OrderBy(x => x.QualificationNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(rc =>
                rolloverCandidateIds.Contains(rc.Id) && rc.IsActive)
            .Select(rc => new RolloverCandidateDto
            {
                Id = rc.Id,
                SourceType = rc.SourceType,
                SourceQualificationId = rc.SourceQualificationId,
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
        var incomingRunIds = incomingRolloverCandidates
            .Select(x => x.RolloverWorkflowRunId)
            .Distinct()
            .ToList();

        var existingWorkflowCandidates = await _context.RolloverWorkflowCandidates
            .Where(x =>
                incomingRunIds.Contains(x.RolloverWorkflowRunId) &&
                incomingCandidateIds.Contains(x.RolloverCandidatesId))
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

    public async Task<IReadOnlyList<RolloverCandidateForExport>> GetRolloverWorkflowCandidatesByRunId(
        Guid workflowRunId,
        CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc => rwc.RolloverWorkflowRunId == workflowRunId
                       && rwc.IncludedInP1Export
                       && !rwc.InvalidatedAt.HasValue)
            .WithAllSourceQualifications(_context)
            .Select(x => new RolloverCandidateForExport
            {
                QAN = x.QualificationReference,
                QualificationTitle = x.QualificationTitle,
                AwardingOrganisation = x.AwardingOrganisation,
                QualificationLevel = x.QualificationLevel,
                QualificationType = x.QualificationType,
                SSA = x.SSA,
                OperationalEndDate = x.OperationalEndDate,
                OfferedInEngland = x.OfferedInEngland,
                FundedInEngland = x.FundedInEngland,
                GLH = x.GLH,
                TQT = x.TQT,
                Pre16 = x.Pre16,
                Age16To18 = x.Age16To18,
                Age18Plus = x.Age18Plus,
                Age19Plus = x.Age19Plus,
                FundingStreamName = x.FundingStreamName,
                FundingApprovalStartDate = x.FundingApprovalStartDate,
                ProposedOutcome = x.PassP1 ? RolloverStatus.Extended.ToString() : RolloverStatus.Excluded.ToString(),
                RolloverStatus = x.PassP1 ? RolloverStatus.Extended : RolloverStatus.Excluded,
                ExclusionReason = x.PassP1 ? x.ExclusionReason : x.P1FailureReason,
                CurrentFundingApprovalEndDate = x.CurrentFundingEndDate,
                ProposedFundingApprovalEndDate = x.ProposedFundingEndDate,
                Comments = string.Empty,
            })
            .OrderBy(x => x.QAN)
            .ToListAsync(cancellationToken);
    }
    public async Task<RolloverWorkflowRun?> GeRolloverWorkflowRunByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowRuns
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<FundingExtensionCandidateValidationContext> GetFundingExtensionValidationContextAsync(
        HashSet<CandidateKey> incomingCandidates,
        CancellationToken cancellationToken)
    {
        var flattened = incomingCandidates
            .Select(k => $"{k.Qan}|{k.FundingStream}")
            .ToList();

        var latestRunId = await GetLatestWorkflowRunIdAsync(cancellationToken);

        if (latestRunId == Guid.Empty)
            throw new InvalidOperationException("No workflow runs exist");

        var candidatesInDb = await _context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .WithAllSourceQualifications(_context)
            .Where(x => flattened.Contains(x.QualificationReference + "|" + x.FundingStreamName))
            .Select(x => new CandidateKey(x.QualificationReference, x.FundingStreamName))
            .ToListAsync(cancellationToken);

        var matchingWorkflowCandidatesInDb = await _context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc =>
                rwc.RolloverWorkflowRunId == latestRunId &&
                !rwc.InvalidatedAt.HasValue)
            .WithAllSourceQualifications(_context)
            .Where(x => flattened.Contains(x.QualificationReference + "|" + x.FundingStreamName))
            .Select(x => new CandidateKey(x.QualificationReference, x.FundingStreamName))
            .ToListAsync(cancellationToken);

        return new FundingExtensionCandidateValidationContext(
            incomingCandidates,
            candidatesInDb.ToHashSet(),
            matchingWorkflowCandidatesInDb.ToHashSet()
        );
    }

    public async Task<List<RolloverCandidateStatusItem>> GetRolloverCandidatesStatusAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .WithAllSourceQualifications(_context)
            .Select(x => new RolloverCandidateStatusItem
            {
                Qan = x.QualificationReference,
                FundingStreamName = x.FundingStreamName,
                RolloverStatus = x.RolloverStatus
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RolloverCandidates>> LoadRolloverCandidateGraphAsync(
        List<CandidateKey> keys,
        CancellationToken cancellationToken)
    {
        var keySet = keys
            .Select(x => x.Qan + "|" + x.FundingStream)
            .ToHashSet();
        var latestRunId = await GetLatestWorkflowRunIdAsync(cancellationToken);

        var sourceContext = await _context.RolloverCandidates
            .Where(x =>
                x.IsActive &&
                _context.RolloverWorkflowCandidates.Any(workflowCandidate =>
                    workflowCandidate.RolloverCandidatesId == x.Id &&
                    workflowCandidate.RolloverWorkflowRunId == latestRunId &&
                    !workflowCandidate.InvalidatedAt.HasValue))
            .WithAllSourceQualifications(_context)
            .Where(x => keySet.Contains(x.QualificationReference + "|" + x.FundingStreamName))
            .Select(x => new
            {
                x.CandidateId,
                x.QualificationReference,
                x.DiscussionQualificationId
            })
            .ToListAsync(cancellationToken);

        var sourceContextByCandidateId = sourceContext
            .ToDictionary(x => x.CandidateId, x => (x.QualificationReference, x.DiscussionQualificationId));

        var ids = sourceContextByCandidateId.Keys.ToList();

        var candidates = await _context.RolloverCandidates
            .Include(x => x.FundingOffer)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var candidate in candidates)
        {
            var context = sourceContextByCandidateId[candidate.Id];
            candidate.SetSourceContext(context.QualificationReference, context.DiscussionQualificationId);
        }

        return candidates;
    }

    public async Task DeleteAllWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        var items = await _context.RolloverWorkflowCandidates
            .ToListAsync(cancellationToken);

        _context.RolloverWorkflowCandidates.RemoveRange(items);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid?> GetLatestWorkflowRunIdAsync(CancellationToken cancellationToken)
    {
        return await _context.RolloverWorkflowRuns
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverCandidateDto>> GetQualificationVersionsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(filters)
            .Select(x => new RolloverCandidateDto
            {
                Id = x.CandidateId,
                SourceType = x.SourceType,
                SourceQualificationId = x.SourceQualificationId,
                FundingOfferId = x.FundingOfferId,
                FundingOfferName = x.FundingStreamName,
                RolloverRound = x.RolloverRound,
                AcademicYear = x.AcademicYear,
                PreviousFundingEndDate = x.PreviousFundingEndDate,
                NewFundingEndDate = x.NewFundingEndDate,
                QualificationNumber = x.QualificationReference,
                QualificationName = x.QualificationTitle
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderLevel>> GetAllLevelsForRolloverQueryBuilderAsync(
        CancellationToken cancellationToken)
    {
        var names = await ActiveSourceCandidates()
            .Where(x => x.QualificationLevel != null)
            .Select(x => x.QualificationLevel!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return names
            .Select(QualificationLevel.FromName)
            .Where(x => x != QualificationLevel.Unspecified)
            .Select(x => new RolloverQueryBuilderLevel { Id = x.Id, Name = x.Name })
            .OrderBy(x => x.Name)
            .ToList();
    }

    public async Task<IEnumerable<RolloverQueryBuilderType>> GetTypesForRolloverQueryBuilderAsync(
        RolloverQueryBuilderTypesRequest requestFilters,
        CancellationToken cancellationToken)
    {
        var names = await ApplyRolloverQueryBuilderFilters(requestFilters)
            .Where(x => x.QualificationType != null)
            .Select(x => x.QualificationType!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return names
            .Select(QualificationType.FromName)
            .Where(x => x != QualificationType.Unknown)
            .Select(x => new RolloverQueryBuilderType { Id = x.Id, Name = x.Name })
            .OrderBy(x => x.Name)
            .ToList();
    }

    public async Task<IEnumerable<RolloverQueryBuilderSectorSubjectArea>> GetSectorSubjectAreasForRolloverQueryBuilderAsync(
        RolloverQueryBuilderSectorSubjectAreaRequest requestFilters,
        CancellationToken cancellationToken)
    {
        var names = await ApplyRolloverQueryBuilderFilters(requestFilters)
            .Where(x => x.SectorSubjectArea != null)
            .Select(x => x.SectorSubjectArea!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return names
            .Select(SectorSubjectArea.FromName)
            .Where(x => x != SectorSubjectArea.NotSpecified)
            .Select(x => new RolloverQueryBuilderSectorSubjectArea { Id = x.Code, Name = x.Name })
            .OrderBy(x => x.Name)
            .ToList();
    }

    public async Task<IEnumerable<RolloverQueryBuilderAwardingOrganisation>> GetAwardingOrganisationsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderAwardingOrganisationsRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(filters)
            .Where(x => x.AwardingOrganisationFilterId != null)
            .Select(x => new RolloverQueryBuilderAwardingOrganisation
            {
                Id = x.AwardingOrganisationId,
                FilterId = x.AwardingOrganisationFilterId,
                Ukprn = x.AwardingOrganisationUkprn,
                RecognitionNumber = x.AwardingOrganisationRecognitionNumber,
                NameLegal = x.AwardingOrganisationNameLegal,
                NameOfqual = x.AwardingOrganisation,
                NameGovUk = x.AwardingOrganisationNameGovUk,
                Name_Dsi = x.AwardingOrganisationNameDsi,
                Acronym = x.AwardingOrganisationAcronym
            })
            .Distinct()
            .OrderBy(x => x.NameOfqual ?? x.NameLegal)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<RolloverCandidateSourceProjection> ActiveSourceCandidates()
    {
        return _context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .WithAllSourceQualifications(_context);
    }

    private IQueryable<RolloverCandidateSourceProjection> ApplyRolloverQueryBuilderFilters(
        IQueryBuilderFilterRequest filters)
    {
        var query = ActiveSourceCandidates();

        if (filters is RolloverQueryBuilderTypesRequest typeFilters)
            query = ApplyLevelFilter(query, typeFilters.LevelIds);

        if (filters is RolloverQueryBuilderSectorSubjectAreaRequest sectorFilters)
        {
            query = ApplyLevelFilter(query, sectorFilters.LevelIds);
            query = ApplyTypeFilter(query, sectorFilters.TypeIds);
        }

        if (filters is RolloverQueryBuilderAwardingOrganisationsRequest organisationFilters)
        {
            query = ApplyLevelFilter(query, organisationFilters.LevelIds);
            query = ApplyTypeFilter(query, organisationFilters.TypeIds);
            query = ApplySectorSubjectAreaFilter(query, organisationFilters.SectorSubjectAreaIds);
        }

        if (filters is RolloverQueryBuilderRequest allFilters)
        {
            query = ApplyLevelFilter(query, allFilters.LevelIds);
            query = ApplyTypeFilter(query, allFilters.TypeIds);
            query = ApplySectorSubjectAreaFilter(query, allFilters.SectorSubjectAreaIds);

            if (allFilters.AwardingOrganisationIds.Count > 0)
            {
                query = query.Where(x =>
                    x.AwardingOrganisationFilterId != null &&
                    allFilters.AwardingOrganisationIds.Contains(x.AwardingOrganisationFilterId));
            }
        }

        return query;
    }

    private static IQueryable<RolloverCandidateSourceProjection> ApplyLevelFilter(
        IQueryable<RolloverCandidateSourceProjection> query,
        IReadOnlyCollection<int> levelIds)
    {
        if (levelIds.Count == 0)
            return query;

        var names = levelIds.Select(x => QualificationLevel.FromId(x).Name).ToList();
        return query.Where(x => x.QualificationLevel != null && names.Contains(x.QualificationLevel));
    }

    private static IQueryable<RolloverCandidateSourceProjection> ApplyTypeFilter(
        IQueryable<RolloverCandidateSourceProjection> query,
        IReadOnlyCollection<int> typeIds)
    {
        if (typeIds.Count == 0)
            return query;

        var names = typeIds.Select(x => QualificationType.FromId(x).Name).ToList();
        return query.Where(x => x.QualificationType != null && names.Contains(x.QualificationType));
    }

    private static IQueryable<RolloverCandidateSourceProjection> ApplySectorSubjectAreaFilter(
        IQueryable<RolloverCandidateSourceProjection> query,
        IReadOnlyCollection<string> sectorSubjectAreaIds)
    {
        if (sectorSubjectAreaIds.Count == 0)
            return query;

        var names = sectorSubjectAreaIds.Select(x => SectorSubjectArea.FromFullCode(x).Name).ToList();
        return query.Where(x => x.SectorSubjectArea != null && names.Contains(x.SectorSubjectArea));
    }
}
