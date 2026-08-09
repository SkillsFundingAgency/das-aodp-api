using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

// Focused coverage for the polymorphic (Ofqual + QAA) rework of RolloverRepository.
// These tests use a real Sqlite in-memory database (not the EF InMemory provider) because
// CreateRolloverWorkflowAsync and DeleteAllWorkflowCandidatesAsync both rely on
// ExecuteDeleteAsync, which the EF InMemory provider does not support.
public class RolloverRepositorySQLLiteTests
{
    private static ApplicationDbContext CreateSqliteDb()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");

        return context;
    }

    private static (Qualification Qualification, QualificationVersions Version, AwardingOrganisation Organisation)
        BuildOfqualQualification(string qan)
    {
        var organisation = new AwardingOrganisation
        {
            Id = Guid.NewGuid(),
            NameOfqual = "Test Awarding Organisation"
        };

        var qualification = new Qualification
        {
            Id = Guid.NewGuid(),
            Qan = qan,
            QualificationName = "Test Ofqual Qualification"
        };

        var version = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualification.Id,
            Qualification = qualification,
            AwardingOrganisationId = organisation.Id,
            Organisation = organisation,
            Status = "Approved",
            Type = "Diploma",
            Ssa = "2.1",
            Level = "3",
            SubLevel = "3",
            EqfLevel = "3",
            OperationalStartDate = new DateTime(2020, 1, 1),
            OperationalEndDate = new DateTime(2030, 1, 1),
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true
        };

        return (qualification, version, organisation);
    }

    private static RegulatedQaaQualification BuildQaaQualification(string aimCode)
    {
        return RegulatedQaaQualification.Create(
            DateTime.UtcNow,
            aimCode,
            "Test QAA Qualification",
            "Test QAA Awarding Body",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5)),
            SectorSubjectArea.Science);
    }

    [Fact]
    public async Task GetRolloverCandidatesWithP1ChecksAsync_ForOfqualCandidate_PopulatesOperationalAndEnglandFieldsFromQualificationVersion()
    {
        // Arrange
        await using var db = CreateSqliteDb();

        var (qualification, version, organisation) = BuildOfqualQualification("QAN001");
        var fundingOffer = new FundingOffer { Id = Guid.NewGuid(), Name = "16-18", DisplayName = "16-18" };
        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual, version.Id, fundingOffer.Id, "2025/26", DateTime.UtcNow);
        var funding = QualificationFundings.Create(
            version.Id, fundingOffer.Id, DateOnly.FromDateTime(DateTime.UtcNow), null);

        db.AwardingOrganisation.Add(organisation);
        db.Qualification.Add(qualification);
        db.QualificationVersions.Add(version);
        db.FundingOffers.Add(fundingOffer);
        db.RolloverCandidates.Add(candidate);
        db.QualificationFundings.Add(funding);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);
        var request = new RolloverCandidateP1CheckRequest(
            candidate.Id, new DateTime(2025, 1, 1), new DateTime(2025, 1, 1), new DateTime(2026, 7, 31));

        // Act
        var result = await sut.GetRolloverCandidatesWithP1ChecksAsync([request], TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(1);
        var checks = result.Single().P1Checks;
        checks.SourceType.ShouldBe(RolloverSourceTypes.Ofqual);
        checks.SourceQualificationId.ShouldBe(version.Id);
        checks.OperationalStartDate.ShouldBe(version.OperationalStartDate);
        checks.OperationalEndDate.ShouldBe(version.OperationalEndDate);
        checks.OfferedInEngland.ShouldBeTrue();
        checks.IntentionToSeekFundingInEngland.ShouldBeTrue();
        checks.FundingStream.ShouldBe(fundingOffer.Name);
    }

    [Fact]
    public async Task GetRolloverCandidatesWithP1ChecksAsync_ForQaaCandidate_LeavesOperationalDatesNullAndEnglandFieldsDefaultedTrue()
    {
        // Arrange
        await using var db = CreateSqliteDb();

        var qaaQualification = BuildQaaQualification("AC001");
        var fundingOffer = new FundingOffer { Id = Guid.NewGuid(), Name = "19+", DisplayName = "19+" };
        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Qaa, qaaQualification.Id, fundingOffer.Id, "2025/26", DateTime.UtcNow);
        var funding = QaaQualificationFunding.Create(
            qaaQualification.Id, fundingOffer.Id,
            DateOnly.FromDateTime(DateTime.UtcNow), null, "Approved", DateTime.UtcNow);

        db.RegulatedQaaQualifications.Add(qaaQualification);
        db.FundingOffers.Add(fundingOffer);
        db.RolloverCandidates.Add(candidate);
        db.QaaQualificationFundings.Add(funding);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);
        var request = new RolloverCandidateP1CheckRequest(
            candidate.Id, new DateTime(2025, 1, 1), new DateTime(2025, 1, 1), new DateTime(2026, 7, 31));

        // Act
        var result = await sut.GetRolloverCandidatesWithP1ChecksAsync([request], TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(1);
        var checks = result.Single().P1Checks;
        checks.SourceType.ShouldBe(RolloverSourceTypes.Qaa);
        checks.SourceQualificationId.ShouldBe(qaaQualification.Id);
        checks.OperationalStartDate.ShouldBeNull();
        checks.OperationalEndDate.ShouldBeNull();
        checks.OfferedInEngland.ShouldBeTrue();
        checks.IntentionToSeekFundingInEngland.ShouldBeTrue();
        checks.FundingStream.ShouldBe(fundingOffer.Name);
    }

    [Fact]
    public async Task GetRolloverCandidatesWithP1ChecksAsync_ForMixedBatch_DoesNotCrossMatchFundingBetweenSourceTypes()
    {
        // Arrange - an Ofqual and a QAA candidate that deliberately share the same
        // SourceQualificationId and FundingOfferId, to prove the funding lookup is
        // genuinely keyed by SourceType and not just by id.
        await using var db = CreateSqliteDb();

        var sharedQualificationId = Guid.NewGuid();
        var fundingOffer = new FundingOffer { Id = Guid.NewGuid(), Name = "16-18", DisplayName = "16-18" };

        var (qualification, version, organisation) = BuildOfqualQualification("QAN002");
        version.Id = sharedQualificationId;
        var ofqualCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual, sharedQualificationId, fundingOffer.Id, "2025/26", DateTime.UtcNow);
        var ofqualFunding = QualificationFundings.Create(
            sharedQualificationId, fundingOffer.Id, DateOnly.FromDateTime(DateTime.UtcNow), new DateOnly(2026, 7, 31));

        var qaaQualification = RegulatedQaaQualification.Create(
            DateTime.UtcNow, "AC002", "Test QAA", "Test Body",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5)),
            SectorSubjectArea.Science);
        var qaaCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Qaa, qaaQualification.Id, fundingOffer.Id, "2025/26", DateTime.UtcNow);
        var qaaFunding = QaaQualificationFunding.Create(
            qaaQualification.Id, fundingOffer.Id, DateOnly.FromDateTime(DateTime.UtcNow), new DateOnly(2027, 7, 31), "Approved", DateTime.UtcNow);

        db.AwardingOrganisation.Add(organisation);
        db.Qualification.Add(qualification);
        db.QualificationVersions.Add(version);
        db.RegulatedQaaQualifications.Add(qaaQualification);
        db.FundingOffers.Add(fundingOffer);
        db.RolloverCandidates.AddRange(ofqualCandidate, qaaCandidate);
        db.QualificationFundings.Add(ofqualFunding);
        db.QaaQualificationFundings.Add(qaaFunding);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);
        var requests = new List<RolloverCandidateP1CheckRequest>
        {
            new(ofqualCandidate.Id, new DateTime(2025, 1, 1), new DateTime(2025, 1, 1), new DateTime(2026, 7, 31)),
            new(qaaCandidate.Id, new DateTime(2025, 1, 1), new DateTime(2025, 1, 1), new DateTime(2026, 7, 31))
        };

        // Act
        var result = await sut.GetRolloverCandidatesWithP1ChecksAsync(requests, TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(2);

        var ofqualResult = result.Single(x => x.Candidate.Id == ofqualCandidate.Id);
        ofqualResult.P1Checks.LatestFundingApprovalEndDate.ShouldBe(new DateTime(2026, 7, 31));

        var qaaResult = result.Single(x => x.Candidate.Id == qaaCandidate.Id);
        qaaResult.P1Checks.LatestFundingApprovalEndDate.ShouldBe(new DateTime(2027, 7, 31));
    }

    [Fact]
    public async Task CreateRolloverWorkflowAsync_CreatesRunCandidatesAndFundingOffersInOneCall()
    {
        // Arrange
        await using var db = CreateSqliteDb();
        var sut = new RolloverRepository(db);

        var (qualification, version, organisation) = BuildOfqualQualification("QAN003");
        var fundingOffer = new FundingOffer { Id = Guid.NewGuid(), Name = "16-18", DisplayName = "16-18" };
        var rolloverCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual, version.Id, fundingOffer.Id, "2025/26", DateTime.UtcNow);

        db.AwardingOrganisation.Add(organisation);
        db.Qualification.Add(qualification);
        db.QualificationVersions.Add(version);
        db.FundingOffers.Add(fundingOffer);
        db.RolloverCandidates.Add(rolloverCandidate);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var now = DateTime.UtcNow;
        var workflowRun = RolloverWorkflowRun.Create(
            "2025/26", Data.Entities.Rollover.Enums.SelectionMethod.QueryBuilder,
            now, now, now.AddYears(1), "test.user", now);
        var workflowCandidate = RolloverWorkflowCandidate.Create(
            workflowRun.Id, rolloverCandidate.Id, RolloverSourceTypes.Ofqual, version.Id,
            fundingOffer.Id, "2025/26", 1, now, null, now);
        var workflowFundingOffer = RolloverWorkflowRunFundingOffer.Create(workflowRun.Id, fundingOffer.Id);

        // Act
        var resultRunId = await sut.CreateRolloverWorkflowAsync(
            workflowRun, [workflowCandidate], [workflowFundingOffer], TestContext.Current.CancellationToken);

        // Assert
        resultRunId.ShouldBe(workflowRun.Id);

        var storedCandidates = await db.RolloverWorkflowCandidates
            .Where(x => x.RolloverWorkflowRunId == workflowRun.Id)
            .ToListAsync(TestContext.Current.CancellationToken);
        storedCandidates.Count.ShouldBe(1);
        storedCandidates.Single().SourceType.ShouldBe(RolloverSourceTypes.Ofqual);

        var storedRun = await db.RolloverWorkflowRuns.FindAsync([workflowRun.Id], TestContext.Current.CancellationToken);
        storedRun.ShouldNotBeNull();

        var storedFundingOffers = await db.RolloverWorkflowRunFundingOffers
            .Where(x => x.RolloverWorkflowRunId == workflowRun.Id)
            .ToListAsync(TestContext.Current.CancellationToken);
        storedFundingOffers.Count.ShouldBe(1);
    }

    [Fact]
    public async Task DeleteAllWorkflowCandidatesAsync_RemovesAllRowsAcrossAllRuns()
    {
        // Arrange
        await using var db = CreateSqliteDb();
        var now = DateTime.UtcNow;

        var candidateA = RolloverWorkflowCandidate.Create(
            Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Ofqual, Guid.NewGuid(),
            Guid.NewGuid(), "2024/25", 1, now, null, now);
        var candidateB = RolloverWorkflowCandidate.Create(
            Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Qaa, Guid.NewGuid(),
            Guid.NewGuid(), "2025/26", 1, now, null, now);

        db.RolloverWorkflowCandidates.AddRange(candidateA, candidateB);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        await sut.DeleteAllWorkflowCandidatesAsync(TestContext.Current.CancellationToken);

        // Assert
        var remaining = await db.RolloverWorkflowCandidates.ToListAsync(TestContext.Current.CancellationToken);
        remaining.ShouldBeEmpty();
    }
}
