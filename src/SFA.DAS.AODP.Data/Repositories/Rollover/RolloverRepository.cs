using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.QueryExtensions;
using SFA.DAS.AODP.Data.ValueObjects;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class RolloverRepository(IApplicationDbContext context) : IRolloverRepository
{
    public async Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken)
    {
        var dbSet = context.RolloverWorkflowCandidates;

        var totalRecords = await dbSet.AsNoTracking().CountAsync(cancellationToken);

        return totalRecords;
    }

    public async Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        return await context.RolloverWorkflowCandidates.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverWorkflowCandidatesP1Checks>> GetRolloverWorkflowCandidatesP1ChecksAsync(CancellationToken cancellationToken)
    {
        var dbSet = context.RolloverWorkflowCandidatesP1Checks;
        var query = await dbSet.AsNoTracking().ToListAsync(cancellationToken);

        return query;
    }

    public async Task UpdateRolloverWorkflowCandidatesAsync(IEnumerable<RolloverWorkflowCandidate> candidates, CancellationToken cancellationToken)
    {
        var list = candidates as IList<RolloverWorkflowCandidate> ?? candidates.ToList();
        if (!list.Any())
            return;

        context.RolloverWorkflowCandidates.UpdateRange(list);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesAsync(CancellationToken cancellationToken)
    {
        return await context.RolloverCandidates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(rc => new RolloverCandidateDto
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

    public async Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, CancellationToken cancellationToken)
    {
        return await context.RolloverCandidates
            .AsNoTracking()
            .Where(rc =>
                rolloverCandidateIds.Contains(rc.Id) && rc.IsActive)
            .Select(rc => new RolloverCandidateDto
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
        context.RolloverWorkflowRuns.Add(workflowRun);
        await context.SaveChangesAsync(cancellationToken);
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

        var existingWorkflowCandidates = await context.RolloverWorkflowCandidates
            .Where(x => incomingCandidateIds.Contains(x.RolloverCandidatesId))
            .ToListAsync(cancellationToken);

        context.RolloverWorkflowCandidates.RemoveRange(existingWorkflowCandidates);

        context.RolloverWorkflowCandidates.AddRange(incomingRolloverCandidates);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateRolloverWorkflowRunFundingOffersAsync(IEnumerable<RolloverWorkflowRunFundingOffer> workflowFundingOffers, CancellationToken cancellationToken)
    {
        context.RolloverWorkflowRunFundingOffers.AddRange(workflowFundingOffers);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RolloverCandidateForExport>> GetRolloverWorkflowCandidatesByRunId(
        Guid workflowRunId,
        CancellationToken cancellationToken)
    {
        return await context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc => rwc.RolloverWorkflowRunId == workflowRunId
                       && rwc.IncludedInP1Export)
            .Select(rwc => new RolloverCandidateForExport
            {
                QAN = rwc.RolloverCandidates.QualificationVersion.Qualification.Qan,
                QualificationTitle = rwc.RolloverCandidates.QualificationVersion.Qualification.QualificationName ?? string.Empty,
                AwardingOrganisation = rwc.RolloverCandidates.QualificationVersion.Organisation.NameOfqual ?? string.Empty,
                QualificationLevel = rwc.RolloverCandidates.QualificationVersion.Level,
                QualificationType = rwc.RolloverCandidates.QualificationVersion.Type,
                SSA = rwc.RolloverCandidates.QualificationVersion.Ssa,
                OperationalEndDate = rwc.RolloverCandidates.QualificationVersion.OperationalEndDate,

                OfferedInEngland = rwc.RolloverCandidates.QualificationVersion.OfferedInEngland,
                FundedInEngland = rwc.RolloverCandidates.QualificationVersion.IntentionToSeekFundingInEngland ?? false,

                GLH = rwc.RolloverCandidates.QualificationVersion.Glh,
                TQT = rwc.RolloverCandidates.QualificationVersion.Tqt,

                Pre16 = rwc.RolloverCandidates.QualificationVersion.PreSixteen ?? false,
                Age16To18 = rwc.RolloverCandidates.QualificationVersion.SixteenToEighteen ?? false,
                Age18Plus = rwc.RolloverCandidates.QualificationVersion.EighteenPlus ?? false,
                Age19Plus = rwc.RolloverCandidates.QualificationVersion.NineteenPlus ?? false,

                FundingStreamName = rwc.RolloverCandidates.FundingOffer.Name,
                FundingApprovalStartDate =
                    context.QualificationFundings
                        .Where(qf =>
                            qf.QualificationVersionId == rwc.RolloverCandidates.QualificationVersionId &&
                            qf.FundingOfferId == rwc.RolloverCandidates.FundingOfferId)
                        .Select(qf => qf.StartDate)
                        .FirstOrDefault(),

                ProposedOutcome = rwc.PassP1 ? RolloverStatus.Extended.ToString() : RolloverStatus.Excluded.ToString(),
                RolloverStatus = rwc.PassP1 ? RolloverStatus.Extended : RolloverStatus.Excluded,
                ExclusionReason = rwc.PassP1 ? rwc.RolloverCandidates.ExclusionReason : rwc.P1FailureReason,

                CurrentFundingApprovalEndDate = rwc.CurrentFundingEndDate,
                ProposedFundingApprovalEndDate = rwc.ProposedFundingEndDate,

                Comments = string.Empty,
            })
            .OrderBy(x => x.QAN)
            .ToListAsync(cancellationToken);
    }
    public async Task<RolloverWorkflowRun?> GeRolloverWorkflowRunByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.RolloverWorkflowRuns
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

        var matchingCandidatesInDB = await context.RolloverCandidates
            .AsNoTracking()
            .Where(rc =>
                flattened.Contains(
                    rc.QualificationVersion.Qualification.Qan + "|" +
                    rc.FundingOffer.Name))
            .Select(rc => new CandidateKey(
                rc.QualificationVersion.Qualification.Qan,
                rc.FundingOffer.Name))
            .ToHashSetAsync(cancellationToken);

        var matchingWorkflowCandidatesInDB = await context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc => rwc.RolloverWorkflowRunId == latestRunId)
            .Where(rwc =>
                flattened.Contains(
                    rwc.QualificationVersion.Qualification.Qan + "|" +
                    rwc.FundingOffer.Name))
            .Select(rwc => new CandidateKey(
                rwc.QualificationVersion.Qualification.Qan,
                rwc.FundingOffer.Name))
            .ToHashSetAsync(cancellationToken);

        return new FundingExtensionCandidateValidationContext(
            incomingCandidates,
            matchingCandidatesInDB,
            matchingWorkflowCandidatesInDB
        );
    }

    public async Task<List<RolloverCandidateStatusItem>> GetRolloverCandidatesStatusAsync(CancellationToken cancellationToken)
    {
        return await context.RolloverCandidates
            .Select(x => new RolloverCandidateStatusItem
            {
                Qan = x.QualificationVersion.Qualification.Qan,
                FundingStreamName = x.FundingOffer.Name,
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

        return await context.RolloverCandidates
            .Include(x => x.QualificationVersion)
                .ThenInclude(v => v.Qualification)
            .Include(x => x.FundingOffer)
            .Where(x =>
                keySet.Contains(
                    x.QualificationVersion.Qualification.Qan + "|" +
                    x.FundingOffer.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAllWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        var items = await context.RolloverWorkflowCandidates
            .ToListAsync(cancellationToken);

        context.RolloverWorkflowCandidates.RemoveRange(items);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid?> GetLatestWorkflowRunIdAsync(CancellationToken cancellationToken)
    {
        return await context.RolloverWorkflowRuns
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQualificationVersion>> GetQualificationVersionsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(filters)
            .Select(qv => new RolloverQualificationVersion
            {
                Id = qv.Id,
                QualificationReference = qv.Qualification.Qan,
                QualificationName = qv.Name ?? qv.Qualification.QualificationName,
                AwardingOrganisationId = qv.AwardingOrganisationId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderLevel>> GetAllLevelsForRolloverQueryBuilderAsync(CancellationToken cancellationToken)
    {
        return await context.RolloverCandidates
            .AsNoTracking()
            .Include(o => o.QualificationVersion)
            .Select(o => new RolloverQueryBuilderLevel
            {
                Id = QualificationLevel.FromName(o.QualificationVersion.Level).Id,
                Name = QualificationLevel.FromName(o.QualificationVersion.Level).Name
            })
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderType>> GetTypesForRolloverQueryBuilderAsync(RolloverQueryBuilderTypesRequest requestFilters,
        CancellationToken cancellationToken)
    {
        var result = ApplyRolloverQueryBuilderFilters(requestFilters);

        return await result
            .Select(o => new RolloverQueryBuilderType { Id = QualificationType.FromName(o.Type).Id, Name = o.Type })
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderSectorSubjectArea>> GetSectorSubjectAreasForRolloverQueryBuilderAsync(RolloverQueryBuilderSectorSubjectAreaRequest requestFilters, CancellationToken cancellationToken)
    {
        var result = ApplyRolloverQueryBuilderFilters(requestFilters);

        return await result
            .Select(o => new RolloverQueryBuilderSectorSubjectArea
            {
                Id = SectorSubjectArea.FromName(o.Ssa).Code,
                Name = SectorSubjectArea.FromName(o.Ssa).Name
            })
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderAwardingOrganisation>> GetAwardingOrganisationsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderAwardingOrganisationsRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(filters)
            .Select(qv => qv.Organisation)
            .Distinct()
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

    private IQueryable<QualificationVersions> ApplyRolloverQueryBuilderFilters(
        IQueryBuilderFilterRequest filters)
    {
        var query = context.RolloverCandidates
            .AsNoTracking()
            .Include(o => o.QualificationVersion)
            .Select(o => o.QualificationVersion);

        if (filters is RolloverQueryBuilderTypesRequest { LevelIds.Count: > 0 } typeFilters)
        {
            query = query.WithLevelFilter(typeFilters.LevelIds);
        }

        if (filters is RolloverQueryBuilderSectorSubjectAreaRequest sectorFilters)
        {
            query = query
                .WithLevelFilter(sectorFilters.LevelIds)
                .WithTypeFilter(sectorFilters.TypeIds);
        }

        if (filters is RolloverQueryBuilderAwardingOrganisationsRequest awardingOrgFilters)
        {
            query = query
                .WithLevelFilter(awardingOrgFilters.LevelIds)
                .WithTypeFilter(awardingOrgFilters.TypeIds)
                .WithSectorSubjectAreaFilter(awardingOrgFilters.SectorSubjectAreaIds);
        }

        if (filters is RolloverQueryBuilderRequest allRolloverFilters)
        {
            query = query.WithAllFilters(
                allRolloverFilters.LevelIds,
                allRolloverFilters.TypeIds,
                allRolloverFilters.SectorSubjectAreaIds,
                allRolloverFilters.AwardingOrganisationIds
            );
        }

        return query;
    }
}