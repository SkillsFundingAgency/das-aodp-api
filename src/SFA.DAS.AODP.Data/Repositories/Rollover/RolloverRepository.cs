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
        return await context.RolloverWorkflowCandidates
            .Include(x => x.RolloverWorkflowRun)
            .ToListAsync(cancellationToken);
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
                FundingOfferName = rc.FundingOffer != null ? rc.FundingOffer.DisplayName : null,
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

    public async Task<IReadOnlyList<RolloverCandidateP1CheckData>> GetRolloverCandidatesWithP1ChecksAsync(
        IReadOnlyCollection<RolloverCandidateP1CheckRequest> requests,
        CancellationToken cancellationToken)
    {
        var rolloverCandidateIds = requests
            .Select(x => x.RolloverCandidateId)
            .ToList();
        var requestsByCandidateId = requests.ToDictionary(x => x.RolloverCandidateId);

        var candidateData = await context.RolloverCandidates
            .AsNoTracking()
            .Where(rc => rolloverCandidateIds.Contains(rc.Id) && rc.IsActive)
            .Select(rc => new
            {
                RolloverCandidatesId = rc.Id,
                rc.QualificationVersionId,
                rc.FundingOfferId,
                FundingStream = rc.FundingOffer.Name,
                rc.AcademicYear,
                rc.RolloverRound,
                rc.PreviousFundingEndDate,
                rc.NewFundingEndDate,
                rc.QualificationVersion.OperationalStartDate,
                rc.QualificationVersion.OperationalEndDate,
                rc.QualificationVersion.OfferedInEngland,
                rc.QualificationVersion.IntentionToSeekFundingInEngland,
                Qan = rc.QualificationVersion.Qualification.Qan,
                HasFunding = context.QualificationFundings.Any(qf =>
                    qf.QualificationVersionId == rc.QualificationVersionId &&
                    qf.FundingOfferId == rc.FundingOfferId),
                LatestFundingApprovalEndDate = context.QualificationFundings
                    .Where(qf =>
                        qf.QualificationVersionId == rc.QualificationVersionId &&
                        qf.FundingOfferId == rc.FundingOfferId)
                    .Max(qf => qf.EndDate)
            })
            .ToListAsync(cancellationToken);

        var qans = candidateData
            .Select(x => x.Qan)
            .Distinct()
            .ToList();

        var defundedQans = await context.DefundingLists
            .AsNoTracking()
            .Where(x => qans.Contains(x.Qan))
            .Select(x => x.Qan)
            .ToHashSetAsync(cancellationToken);

        var pldnsByQan = (await context.Pldns
                .AsNoTracking()
                .Where(x => qans.Contains(x.Qan))
                .ToListAsync(cancellationToken))
            .GroupBy(x => x.Qan)
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(x => x.ImportDate).First());

        return candidateData
            .Select(candidate =>
            {
                pldnsByQan.TryGetValue(candidate.Qan, out var pldns);
                var request = requestsByCandidateId[candidate.RolloverCandidatesId];

                return new RolloverCandidateP1CheckData(
                    new RolloverCandidateDto
                    {
                        Id = candidate.RolloverCandidatesId,
                        QualificationVersionId = candidate.QualificationVersionId,
                        FundingOfferId = candidate.FundingOfferId,
                        AcademicYear = candidate.AcademicYear,
                        RolloverRound = candidate.RolloverRound,
                        PreviousFundingEndDate = candidate.PreviousFundingEndDate,
                        NewFundingEndDate = candidate.NewFundingEndDate
                    },
                    new RolloverWorkflowCandidatesP1Checks
                    {
                        RolloverCandidatesId = candidate.RolloverCandidatesId,
                        QualificationVersionId = candidate.QualificationVersionId,
                        FundingOfferId = candidate.FundingOfferId,
                        FundingStream = candidate.HasFunding ? candidate.FundingStream : null,
                        AcademicYear = candidate.AcademicYear,
                        RolloverRound = candidate.RolloverRound,
                        FundingEndDateThreshold = request.FundingEndDateEligibilityThreshold,
                        OperationalEndDateThreshold =
                            request.OperationalEndDateEligibilityThreshold,
                        MaximumApprovalEndDate = request.MaximumApprovalFundingEndDate,
                        LatestFundingApprovalEndDate = candidate.LatestFundingApprovalEndDate?
                            .ToDateTime(TimeOnly.MinValue),
                        OperationalStartDate = candidate.OperationalStartDate,
                        OperationalEndDate = candidate.OperationalEndDate,
                        OfferedInEngland = candidate.OfferedInEngland,
                        IntentionToSeekFundingInEngland =
                            candidate.IntentionToSeekFundingInEngland ?? false,
                        IsOnDefundingList = defundedQans.Contains(candidate.Qan),
                        Age1416 = pldns?.Pldns14To16,
                        Age1619 = pldns?.Pldns16To19,
                        LocalFlexibilities = pldns?.LocalFlex,
                        LegalEntitlementL2L3 = pldns?.LegalEntitlementL2L3,
                        LegalEntitlementEnglishandMaths = pldns?.LegalEntitlementEngMaths,
                        DigitalEntitlement = pldns?.DigitalEntitlement,
                        ESFL3L4 = pldns?.EsfL3L4,
                        AdvancedLearnerLoans = pldns?.Loans,
                        LifelongLearningEntitlement = pldns?.LifelongLearning,
                        L3FreeCoursesForJobs = pldns?.Level3FCoursesForJobs,
                        CoF = pldns?.Cof
                    });
            })
            .ToList();
    }

    public async Task<Guid> CreateRolloverWorkflowAsync(
        RolloverWorkflowRun workflowRun,
        IReadOnlyCollection<RolloverWorkflowCandidate> workflowCandidates,
        IReadOnlyCollection<RolloverWorkflowRunFundingOffer> workflowFundingOffers,
        CancellationToken cancellationToken)
    {
        var incomingCandidateIds = workflowCandidates
            .Select(x => x.RolloverCandidatesId)
            .ToList();

        await using var transaction = await context.StartTransactionAsync();
        try
        {
            await context.RolloverWorkflowCandidates
                .Where(x => !incomingCandidateIds.Contains(x.RolloverCandidatesId))
                .ExecuteDeleteAsync(cancellationToken);

            context.RolloverWorkflowRuns.Add(workflowRun);
            context.RolloverWorkflowCandidates.AddRange(workflowCandidates);
            context.RolloverWorkflowRunFundingOffers.AddRange(workflowFundingOffers);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return workflowRun.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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

    public async Task<IEnumerable<RolloverCandidateDto>> GetQualificationVersionsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(filters)
            .Select(rc => new RolloverCandidateDto
            {
                Id = rc.Id,
                QualificationVersionId = rc.QualificationVersionId,
                FundingOfferId = rc.FundingOfferId,
                FundingOfferName = rc.FundingOffer.DisplayName,
                RolloverRound = rc.RolloverRound,
                AcademicYear = rc.AcademicYear,
                PreviousFundingEndDate = rc.PreviousFundingEndDate,
                NewFundingEndDate = rc.NewFundingEndDate,
                QualificationNumber = rc.QualificationVersion.Qualification.Qan,
                QualificationName = rc.QualificationVersion.Qualification.QualificationName
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
            .Select(o => new RolloverQueryBuilderType { Id = QualificationType.FromName(o.QualificationVersion.Type).Id, Name = o.QualificationVersion.Type })
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderSectorSubjectArea>> GetSectorSubjectAreasForRolloverQueryBuilderAsync(RolloverQueryBuilderSectorSubjectAreaRequest requestFilters, CancellationToken cancellationToken)
    {
        var result = ApplyRolloverQueryBuilderFilters(requestFilters);

        return await result
            .Select(o => new RolloverQueryBuilderSectorSubjectArea
            {
                Id = SectorSubjectArea.FromName(o.QualificationVersion.Ssa).Code,
                Name = SectorSubjectArea.FromName(o.QualificationVersion.Ssa).Name
            })
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RolloverQueryBuilderAwardingOrganisation>> GetAwardingOrganisationsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderAwardingOrganisationsRequest filters,
        CancellationToken cancellationToken)
    {
        return await ApplyRolloverQueryBuilderFilters(filters)
            .Select(qv => qv.QualificationVersion.Organisation)
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

    private IQueryable<RolloverCandidates> ApplyRolloverQueryBuilderFilters(
        IQueryBuilderFilterRequest filters)
    {
        var query = context.RolloverCandidates
            .AsNoTracking()
            .Include(o => o.QualificationVersion)
            .ThenInclude(o => o.Organisation)
            .Select(o => o);

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
    public async Task<RolloverStartSummary> GetRolloverStartSummaryAsync(string academicYear, CancellationToken cancellationToken) 
    {
        var candidates = await context.RolloverCandidates
            .Where(x => x.AcademicYear == academicYear)
            .ToListAsync(cancellationToken);

        return new RolloverStartSummary
        {
            TotalCandidatesCount = candidates.Count,
            CandidatesEligibleCount = candidates.Count(x => x.RolloverStatus == RolloverStatus.Extended),
            CandidatesIneligibleCount = candidates.Count(x => x.RolloverStatus == RolloverStatus.Excluded),
            CandidatesRemainingCount = candidates.Count(x => x.RolloverStatus == RolloverStatus.NeedsReview)
        };
    }
}
