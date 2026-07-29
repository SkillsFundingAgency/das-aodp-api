using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Data.ValueObjects;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverQueryBuilderRepositoryTests : UnitTest
{
    [Fact]
    public async Task GetSectorSubjectAreasForRolloverQueryBuilderAsync_TranslatesQaaProjectionToSql()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync(CancellationToken);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync(CancellationToken);

        var qaaQualification = RegulatedQaaQualification.Create(
            new DateTime(2026, 1, 1),
            "Z1234567",
            "Access to HE Diploma",
            "QAA Awarding Body",
            new DateOnly(2025, 8, 1),
            new DateOnly(2026, 7, 31),
            SectorSubjectArea.Science);
        var qaaQualificationId = Guid.NewGuid();
        typeof(RegulatedQaaQualification)
            .GetProperty(nameof(RegulatedQaaQualification.Id))!
            .SetValue(qaaQualification, qaaQualificationId);
        var fundingOffer = new FundingOffer
        {
            Id = Guid.NewGuid(),
            Name = "Age1619",
            DisplayName = "Age 16-19"
        };
        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Qaa,
            qaaQualificationId,
            fundingOffer.Id,
            "2025/26",
            new DateTime(2026, 1, 1));
        candidate.FundingOffer = fundingOffer;

        context.RegulatedQaaQualifications.Add(qaaQualification);
        context.RolloverCandidates.Add(candidate);
        await context.SaveChangesAsync(CancellationToken);

        var sut = new RolloverRepository(context);
        var result = await sut.GetSectorSubjectAreasForRolloverQueryBuilderAsync(
            new RolloverQueryBuilderSectorSubjectAreaRequest([], []),
            CancellationToken);

        result.Single().Name.ShouldBe(SectorSubjectArea.Science.Name);
    }

    [Fact]
    public async Task GetAllLevelsForRolloverQueryBuilderAsync_WhenCandidatesContainDuplicateLevels_ReturnsDistinctLevels()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedCandidates(context);
        var sut = new RolloverRepository(context);

        // Act
        var result = (await sut.GetAllLevelsForRolloverQueryBuilderAsync(CancellationToken)).ToList();

        // Assert
        result.ShouldBeEquivalentTo(new List<RolloverQueryBuilderLevel>
        {
            new RolloverQueryBuilderLevel { Id = QualificationLevel.Level1.Id, Name = QualificationLevel.Level1.Name },
            new RolloverQueryBuilderLevel { Id = QualificationLevel.Level2.Id, Name = QualificationLevel.Level2.Name }
        });
    }

    [Fact]
    public async Task GetTypesForRolloverQueryBuilderAsync_WhenLevelIsSelected_ReturnsTypesForThatLevelOnly()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedCandidates(context);
        var sut = new RolloverRepository(context);
        var request = new RolloverQueryBuilderTypesRequest([QualificationLevel.Level1.Id]);

        // Act
        var result = (await sut.GetTypesForRolloverQueryBuilderAsync(request, CancellationToken)).ToList();

        // Assert
        result.ShouldBeEquivalentTo(new List<RolloverQueryBuilderType>
        {
            new RolloverQueryBuilderType
            {
                Id = QualificationType.AccessToHigherEducation.Id,
                Name = QualificationType.AccessToHigherEducation.Name
            },
            new RolloverQueryBuilderType
            {
                Id = QualificationType.AdvancedExtensionAward.Id,
                Name = QualificationType.AdvancedExtensionAward.Name
            }
        });
    }

    [Fact]
    public async Task GetTypesForRolloverQueryBuilderAsync_WhenNoLevelIsSelected_ReturnsAllDistinctTypes()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedCandidates(context);
        var sut = new RolloverRepository(context);
        var request = new RolloverQueryBuilderTypesRequest([]);

        // Act
        var result = (await sut.GetTypesForRolloverQueryBuilderAsync(request, CancellationToken)).ToList();

        // Assert
        result.Count.ShouldBe(2);
        result.Select(x => x.Id).ShouldBe([QualificationType.AccessToHigherEducation.Id, QualificationType.AdvancedExtensionAward.Id], ignoreOrder: true);
    }

    [Fact]
    public async Task GetSectorSubjectAreasForRolloverQueryBuilderAsync_WhenLevelAndTypeAreSelected_ReturnsMatchingAreasOnly()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedCandidates(context);
        var sut = new RolloverRepository(context);
        var request = new RolloverQueryBuilderSectorSubjectAreaRequest(
            [QualificationLevel.Level1.Id],
            [QualificationType.AdvancedExtensionAward.Id]);

        // Act
        var result = (await sut.GetSectorSubjectAreasForRolloverQueryBuilderAsync(request, CancellationToken)).Single();

        // Assert
        result.ShouldBeEquivalentTo(new RolloverQueryBuilderSectorSubjectArea
        {
            Id = SectorSubjectArea.Science.Code,
            Name = SectorSubjectArea.Science.Name
        });
    }

    [Fact]
    public async Task GetAwardingOrganisationsForRolloverQueryBuilderAsync_WhenFiltersAreSelected_ReturnsCompleteMatchingOrganisation()
    {
        // Arrange
        await using var context = CreateContext();
        var seeded = await SeedCandidates(context);
        var sut = new RolloverRepository(context);
        var request = new RolloverQueryBuilderAwardingOrganisationsRequest(
            [QualificationLevel.Level1.Id],
            [QualificationType.AccessToHigherEducation.Id],
            [SectorSubjectArea.MedicineAndDentistry.Code]);

        // Act
        var result = (await sut.GetAwardingOrganisationsForRolloverQueryBuilderAsync(request, CancellationToken)).Single();

        // Assert
        result.ShouldBeEquivalentTo(new RolloverQueryBuilderAwardingOrganisation
        {
            Id = seeded.FirstOrganisation.Id,
            FilterId = seeded.FirstOrganisation.RecognitionNumber,
            Ukprn = seeded.FirstOrganisation.Ukprn,
            RecognitionNumber = seeded.FirstOrganisation.RecognitionNumber,
            NameLegal = seeded.FirstOrganisation.NameLegal,
            NameOfqual = seeded.FirstOrganisation.NameOfqual,
            NameGovUk = seeded.FirstOrganisation.NameGovUk,
            Name_Dsi = seeded.FirstOrganisation.Name_Dsi,
            Acronym = seeded.FirstOrganisation.Acronym
        });
    }

    [Fact]
    public async Task GetQualificationVersionsForRolloverQueryBuilderAsync_WhenAllFiltersMatch_ReturnsCompleteCandidateMapping()
    {
        // Arrange
        await using var context = CreateContext();
        var seeded = await SeedCandidates(context);
        var sut = new RolloverRepository(context);
        var request = new RolloverQueryBuilderRequest
        {
            LevelIds = [QualificationLevel.Level1.Id],
            TypeIds = [QualificationType.AccessToHigherEducation.Id],
            SectorSubjectAreaIds = [SectorSubjectArea.MedicineAndDentistry.Code],
            AwardingOrganisationIds = [seeded.FirstOrganisation.RecognitionNumber!]
        };

        // Act
        var result = (await sut.GetQualificationVersionsForRolloverQueryBuilderAsync(request, CancellationToken)).Single();

        // Assert
        result.ShouldBeEquivalentTo(new RolloverCandidateDto
        {
            Id = seeded.FirstCandidate.Id,
            SourceType = RolloverSourceTypes.Ofqual,
            SourceQualificationId = seeded.FirstCandidate.SourceQualificationId,
            QualificationNumber = seeded.FirstVersion.Qualification.Qan,
            QualificationName = seeded.FirstVersion.Qualification.QualificationName,
            FundingOfferId = seeded.FirstCandidate.FundingOfferId,
            FundingOfferName = seeded.FirstCandidate.FundingOffer.DisplayName,
            AcademicYear = seeded.FirstCandidate.AcademicYear,
            RolloverRound = seeded.FirstCandidate.RolloverRound,
            PreviousFundingEndDate = seeded.FirstCandidate.PreviousFundingEndDate,
            NewFundingEndDate = seeded.FirstCandidate.NewFundingEndDate
        });
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"RolloverQueryBuilder_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    private async Task<SeededCandidates> SeedCandidates(ApplicationDbContext context)
    {
        var firstOrganisation = CreateOrganisation("RN-ONE", "First Organisation", 100001);
        var secondOrganisation = CreateOrganisation("RN-TWO", "Second Organisation", 100002);

        var firstCandidate = CreateCandidate(
            QualificationLevel.Level1,
            QualificationType.AccessToHigherEducation,
            SectorSubjectArea.MedicineAndDentistry,
            firstOrganisation,
            "QAN-001",
            "First qualification",
            "First offer");
        var secondCandidate = CreateCandidate(
            QualificationLevel.Level2,
            QualificationType.AccessToHigherEducation,
            SectorSubjectArea.MedicineAndDentistry,
            secondOrganisation,
            "QAN-002",
            "Second qualification",
            "Second offer");
        var thirdCandidate = CreateCandidate(
            QualificationLevel.Level1,
            QualificationType.AdvancedExtensionAward,
            SectorSubjectArea.Science,
            secondOrganisation,
            "QAN-003",
            "Third qualification",
            "Third offer");

        context.QualificationVersions.AddRange(firstCandidate.Version, secondCandidate.Version, thirdCandidate.Version);
        context.RolloverCandidates.AddRange(firstCandidate.Candidate, secondCandidate.Candidate, thirdCandidate.Candidate);
        await context.SaveChangesAsync(CancellationToken);

        return new SeededCandidates(firstCandidate.Candidate, firstCandidate.Version, firstOrganisation);
    }

    private static CandidateSeed CreateCandidate(
        QualificationLevel level,
        QualificationType type,
        SectorSubjectArea sectorSubjectArea,
        AwardingOrganisation organisation,
        string qualificationNumber,
        string qualificationName,
        string offerName)
    {
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var qualificationId = Guid.NewGuid();
        var candidate = RolloverCandidates.CreateInitialRound(
            qualificationVersionId,
            fundingOfferId,
            "2026/27",
            new DateTime(2026, 7, 1));

        var qualificationVersion = new QualificationVersions
        {
            Id = qualificationVersionId,
            Level = level.Name,
            Type = type.Name,
            Ssa = sectorSubjectArea.Name,
            Status = "Available",
            SubLevel = level.Name,
            EqfLevel = level.Name,
            AwardingOrganisationId = organisation.Id,
            Organisation = organisation,
            QualificationId = qualificationId,
            Qualification = new Qualification
            {
                Id = qualificationId,
                Qan = qualificationNumber,
                QualificationName = qualificationName
            }
        };
        candidate.FundingOffer = new FundingOffer
        {
            Id = fundingOfferId,
            Name = offerName,
            DisplayName = offerName
        };

        return new CandidateSeed(candidate, qualificationVersion);
    }

    private static AwardingOrganisation CreateOrganisation(string recognitionNumber, string name, int ukprn)
        => new()
        {
            Id = Guid.NewGuid(),
            Ukprn = ukprn,
            RecognitionNumber = recognitionNumber,
            NameLegal = name,
            NameOfqual = $"{name} Ofqual",
            NameGovUk = $"{name} Gov.uk",
            Name_Dsi = $"{name} DSI",
            Acronym = name[..1]
        };

    private sealed record CandidateSeed(RolloverCandidates Candidate, QualificationVersions Version);

    private sealed record SeededCandidates(
        RolloverCandidates FirstCandidate,
        QualificationVersions FirstVersion,
        AwardingOrganisation FirstOrganisation);
}
