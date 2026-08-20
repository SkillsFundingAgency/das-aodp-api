using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.ValueObjects;
using SFA.DAS.AODP.Models.Rollover;
using PldnsEntity = SFA.DAS.AODP.Data.Entities.Import.Pldns;

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
        // Scoped to the latest run so this always agrees with GetAllRolloverWorkflowCandidatesAsync,
        // which the same screen uses to show the actual candidates behind this count.
        var latestRunId = await GetLatestWorkflowRunIdAsync(cancellationToken) ?? Guid.Empty;

        var totalRecords = await _context.RolloverWorkflowCandidates
            .AsNoTracking()
            .CountAsync(x => x.RolloverWorkflowRunId == latestRunId && !x.InvalidatedAt.HasValue, cancellationToken);

        return totalRecords;
    }

    public async Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken)
    {
        // Scoped to the latest run
        var latestRunId = await GetLatestWorkflowRunIdAsync(cancellationToken) ?? Guid.Empty;

        return await _context.RolloverWorkflowCandidates
            .Include(x => x.RolloverWorkflowRun)
            .Where(x => x.RolloverWorkflowRunId == latestRunId && !x.InvalidatedAt.HasValue)
            .ToListAsync(cancellationToken);
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
                QualificationName = x.QualificationTitle,
                AcademicYear = x.AcademicYear
            })
            .OrderBy(x => x.QualificationNumber)
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

        var candidateData = await _context.RolloverCandidates
            .Where(rc => rolloverCandidateIds.Contains(rc.Id) && rc.IsActive)
            .WithAllSourceQualifications(_context)
            .ToListAsync(cancellationToken);

        var ofqualQualificationIds = candidateData
            .Where(x => x.SourceType == RolloverSourceTypes.Ofqual)
            .Select(x => x.SourceQualificationId)
            .Distinct()
            .ToList();
        var qaaQualificationIds = candidateData
            .Where(x => x.SourceType == RolloverSourceTypes.Qaa)
            .Select(x => x.SourceQualificationId)
            .Distinct()
            .ToList();
        var fundingOfferIds = candidateData
            .Select(x => x.FundingOfferId)
            .Distinct()
            .ToList();

        var ofqualFundings = await _context.QualificationFundings
            .AsNoTracking()
            .Where(qf =>
                ofqualQualificationIds.Contains(qf.QualificationVersionId) &&
                fundingOfferIds.Contains(qf.FundingOfferId))
            .Select(qf => new { QualificationId = qf.QualificationVersionId, qf.FundingOfferId, qf.EndDate })
            .ToListAsync(cancellationToken);

        var qaaFundings = await _context.QaaQualificationFundings
            .AsNoTracking()
            .Where(qf =>
                qaaQualificationIds.Contains(qf.QaaQualificationId) &&
                fundingOfferIds.Contains(qf.FundingOfferId))
            .Select(qf => new { QualificationId = qf.QaaQualificationId, qf.FundingOfferId, qf.EndDate })
            .ToListAsync(cancellationToken);

        var latestFundingEndDateByKey = ofqualFundings
            .Select(x => (Key: (SourceType: RolloverSourceTypes.Ofqual, x.QualificationId, x.FundingOfferId), x.EndDate))
            .Concat(qaaFundings.Select(x => (Key: (SourceType: RolloverSourceTypes.Qaa, x.QualificationId, x.FundingOfferId), x.EndDate)))
            .GroupBy(x => x.Key)
            .ToDictionary(g => g.Key, g => g.Max(x => x.EndDate));

        var qans = candidateData
            .Select(x => x.QualificationReference)
            .Distinct()
            .ToList();

        var defundedQans = await _context.DefundingLists
            .AsNoTracking()
            .Where(x => qans.Contains(x.Qan))
            .Select(x => x.Qan)
            .ToHashSetAsync(cancellationToken);

        var pldnsByQan = (await _context.Pldns
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
                pldnsByQan.TryGetValue(candidate.QualificationReference, out var pldns);
                var request = requestsByCandidateId[candidate.CandidateId];
                var fundingKey = (candidate.SourceType, candidate.SourceQualificationId, candidate.FundingOfferId);
                var hasFunding = latestFundingEndDateByKey.TryGetValue(fundingKey, out var latestFundingApprovalEndDate);

                return new RolloverCandidateP1CheckData(
                    new RolloverCandidateDto
                    {
                        Id = candidate.CandidateId,
                        SourceType = candidate.SourceType,
                        SourceQualificationId = candidate.SourceQualificationId,
                        FundingOfferId = candidate.FundingOfferId,
                        FundingOfferName = candidate.FundingStreamName,
                        QualificationNumber = candidate.QualificationReference,
                        QualificationName = candidate.QualificationTitle,
                        AcademicYear = candidate.AcademicYear,
                        RolloverRound = candidate.RolloverRound,
                        PreviousFundingEndDate = candidate.PreviousFundingEndDate,
                        NewFundingEndDate = candidate.NewFundingEndDate
                    },
                    new RolloverWorkflowCandidatesP1Checks
                    {
                        RolloverCandidatesId = candidate.CandidateId,
                        SourceType = candidate.SourceType,
                        SourceQualificationId = candidate.SourceQualificationId,
                        FundingOfferId = candidate.FundingOfferId,
                        FundingStream = hasFunding ? candidate.FundingStreamName : null,
                        AcademicYear = candidate.AcademicYear,
                        RolloverRound = candidate.RolloverRound,
                        FundingEndDateThreshold = request.FundingEndDateEligibilityThreshold,
                        OperationalEndDateThreshold =
                            request.OperationalEndDateEligibilityThreshold,
                        MaximumApprovalEndDate = request.MaximumApprovalFundingEndDate,
                        LatestFundingApprovalEndDate = latestFundingApprovalEndDate?
                            .ToDateTime(TimeOnly.MinValue),
                        OperationalStartDate = candidate.OperationalStartDate,
                        OperationalEndDate = candidate.OperationalEndDate,
                        OfferedInEngland = candidate.OfferedInEngland,
                        IntentionToSeekFundingInEngland = candidate.IntentionToSeekFundingInEngland,
                        IsOnDefundingList = defundedQans.Contains(candidate.QualificationReference),
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

        // Explicit transaction as we are calling ExecuteDeleteAsync for better set based delete operations,
        // this doesn't implicitly create transactions and runs immediately, therefore an explicit transaction is needed.
        // This is so we can ensure a proper unit of work around the delete and the subsequent inserts such that if the delete fails then we roll back the entire thing.
        await using var transaction = await _context.StartTransactionAsync();
        try
        {
            // ExecuteDeleteAsync executes immediately! and does not use implicit transactions.
            await _context.RolloverWorkflowCandidates
                .Where(x => EF.Constant(incomingCandidateIds).Contains(x.RolloverCandidatesId))
                .ExecuteDeleteAsync(cancellationToken);

            _context.RolloverWorkflowRuns.Add(workflowRun);
            _context.RolloverWorkflowCandidates.AddRange(workflowCandidates);
            _context.RolloverWorkflowRunFundingOffers.AddRange(workflowFundingOffers);

            await _context.SaveChangesAsync(cancellationToken);
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
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RolloverCandidateForExport>> GetRolloverWorkflowCandidatesByRunId(
        Guid workflowRunId,
        CancellationToken cancellationToken)
    {
        var candidates = await _context.RolloverWorkflowCandidates
            .AsNoTracking()
            .Where(rwc => rwc.RolloverWorkflowRunId == workflowRunId
                       && rwc.IncludedInP1Export
                       && !rwc.InvalidatedAt.HasValue)
            .WithAllSourceQualifications(_context)
            .ToListAsync(cancellationToken);

        // Batch-fetch PLDNS rows for every QAN in one query instead of a correlated
        // subquery per candidate row - avoids re-scanning Pldns once per row.
        var qans = candidates.Select(x => x.QualificationReference).Distinct().ToList();

        var pldnsByQan = (await _context.Pldns
                .AsNoTracking()
                .Where(p => qans.Contains(p.Qan))
                .ToListAsync(cancellationToken))
            .GroupBy(p => p.Qan)
            .ToDictionary(g => g.Key, g => g.First());

        return candidates
            .Select(x =>
            {
                pldnsByQan.TryGetValue(x.QualificationReference, out var pldns);

                return new RolloverCandidateForExport
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
                    Pldns = GetPldnsDateForFundingStream(pldns, x.FundingOfferId)
                };
            })
            .OrderBy(x => x.QAN)
            .ToList();
    }

    private static DateTime? GetPldnsDateForFundingStream(PldnsEntity? pldns, Guid fundingOfferId)
    {
        if (pldns == null)
        {
            return null;
        }

        if (fundingOfferId == FundingStream.Age1416.Id) return pldns.Pldns14To16;
        if (fundingOfferId == FundingStream.Age1619.Id) return pldns.Pldns16To19;
        if (fundingOfferId == FundingStream.LocalFlexibilities.Id) return pldns.LocalFlex;
        if (fundingOfferId == FundingStream.LegalEntitlementL2L3.Id) return pldns.LegalEntitlementL2L3;
        if (fundingOfferId == FundingStream.LegalEntitlementEnglishAndMaths.Id) return pldns.LegalEntitlementEngMaths;
        if (fundingOfferId == FundingStream.DigitalEntitlement.Id) return pldns.DigitalEntitlement;
        if (fundingOfferId == FundingStream.AdvancedLearnerLoans.Id) return pldns.Loans;
        if (fundingOfferId == FundingStream.LifelongLearningEntitlement.Id) return pldns.LifelongLearning;
        if (fundingOfferId == FundingStream.FreeCoursesForJobs.Id) return pldns.Level3FCoursesForJobs;

        return null;
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
        await _context.RolloverWorkflowCandidates.ExecuteDeleteAsync(cancellationToken);
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
    public async Task<RolloverStartSummary> GetRolloverStartSummaryAsync(string academicYear, CancellationToken cancellationToken)
    {
        var candidates = await _context.RolloverCandidates
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