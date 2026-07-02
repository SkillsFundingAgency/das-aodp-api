using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.QueryExtensions;
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
        query = query
            .WhereEligibleForFunding()
            .WhereLatestVersionPerQualification();

        if (filters.LevelIds.Count > 0)
        {
            query = query.Where(qv => filters.LevelIds.Select(o => QualificationLevel.FromId(o).ToString()).Contains(qv.Level));
        }

        if (filters.TypeIds.Count > 0)
        {
            query = query.Where(qv => filters.TypeIds.Select(o => QualificationType.FromId(o).ToString()).Contains(qv.Type));
        }

        if (filters.SectorSubjectAreaIds.Count > 0)
        {
            query = query.Where(qv => filters.SectorSubjectAreaIds.Select(o => SectorSubjectArea.FromFullCode(o).ToString()).Contains(qv.Ssa));
        }

        if (includeAwardingOrganisations && filters.AwardingOrganisationIds.Count > 0)
        {
            query = query.Where(qv => filters.AwardingOrganisationIds.Contains(qv.AwardingOrganisationId));
        }

        return query;
    }
}

public record QualificationLevel
{
    public static readonly QualificationLevel EntryLevel = new(0, "Entry level");
    public static readonly QualificationLevel Level1 = new(1, "Level 1");
    public static readonly QualificationLevel Level1Or2 = new(12, "Level 1/Level 2");
    public static readonly QualificationLevel Level2 = new(2, "Level 2");
    public static readonly QualificationLevel Level3 = new(3, "Level 3");
    public static readonly QualificationLevel Level4 = new(4, "Level 4");
    public static readonly QualificationLevel Level5 = new(5, "Level 5");
    public static readonly QualificationLevel Level6 = new(6, "Level 6");
    public static readonly QualificationLevel Level7 = new(7, "Level 7");
    public static readonly QualificationLevel Unspecified = new(99, "Unspecified");

    public int Id { get; }
    public string Name { get; set; } = null!;

    public QualificationLevel(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static readonly IReadOnlyCollection<QualificationLevel> All = new List<QualificationLevel>
    {
        EntryLevel, Level1, Level1Or2, Level2, Level3, Level4, Level5, Level6, Level7
    }.OrderBy(o => o.Name).ToList();

    public static QualificationLevel FromId(int id) => All.FirstOrDefault(x => x.Id == id) ?? Unspecified;

    public override string ToString() => Name;
}

public record QualificationType
{
    public static readonly QualificationType None = new(0, "None");
    public static readonly QualificationType AccessToHigherEducation = new(1, "Access to Higher Education");
    public static readonly QualificationType AdvancedExtensionAward = new(2, "Advanced Extension Award");
    public static readonly QualificationType AlternativeAcademicQualification = new(3, "Alternative Academic Qualification");
    public static readonly QualificationType DigitalFunctionalSkillsQualification = new(4, "Digital Functional Skills Qualification");
    public static readonly QualificationType EnglishForSpeakersOfOtherLanguages = new(5, "English For Speakers of Other Languages");
    public static readonly QualificationType EssentialDigitalSkills = new(6, "Essential Digital Skills");
    public static readonly QualificationType FunctionalSkills = new(7, "Functional Skills");
    public static readonly QualificationType GCEAlevel = new(8, "GCE A Level");
    public static readonly QualificationType GCEASLevel = new(9, "GCE AS Level");
    public static readonly QualificationType GCSE9To1 = new(10, "GCSE (9 to 1)");
    public static readonly QualificationType OccupationalQualification = new(11, "Occupational Qualification");
    public static readonly QualificationType OtherGeneralQualification = new(12, "Other General Qualification");
    public static readonly QualificationType OtherLifeSkillsQualification = new(13, "Other Life Skills Qualification");
    public static readonly QualificationType OtherVocationalQualification = new(14, "Other Vocational Qualification");
    public static readonly QualificationType PerformingArtsGradedExamination = new(15, "Performing Arts Graded Examination");
    public static readonly QualificationType PrincipalLearning = new(16, "Principal Learning");
    public static readonly QualificationType Project = new(17, "Project");
    public static readonly QualificationType TechnicalOccupationQualification = new(18, "Technical Occupation Qualification");
    public static readonly QualificationType TechnicalQualification = new(19, "Technical Qualification");
    public static readonly QualificationType VocationallyRelatedQualification = new(20, "Vocationally-Related Qualification");
    public static readonly QualificationType Unknown = new(99, "Unknown");

    public int Id { get; }
    public string Name { get; }

    private QualificationType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static readonly IReadOnlyCollection<QualificationType> All = new List<QualificationType>
    {
        AccessToHigherEducation,
        AdvancedExtensionAward,
        AlternativeAcademicQualification,
        DigitalFunctionalSkillsQualification,
        EnglishForSpeakersOfOtherLanguages,
        EssentialDigitalSkills,
        FunctionalSkills,
        GCEAlevel,
        GCEASLevel,
        GCSE9To1,
        OccupationalQualification,
        OtherGeneralQualification,
        OtherLifeSkillsQualification,
        OtherVocationalQualification,
        PerformingArtsGradedExamination,
        PrincipalLearning,
        Project,
        TechnicalOccupationQualification,
        TechnicalQualification,
        VocationallyRelatedQualification
    }.OrderBy(o => o.Name).ToList();

    public static QualificationType FromId(int id) => All.FirstOrDefault(x => x.Id == id) ?? Unknown;

    public override string ToString() => Name;
}