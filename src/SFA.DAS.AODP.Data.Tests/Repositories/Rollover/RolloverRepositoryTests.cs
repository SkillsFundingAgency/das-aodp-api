using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover.Enums;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

// NOTE: CreateDb uses Sqlite, not the EF InMemory provider. This is required because
// DeleteAllWorkflowCandidatesAsync and CreateRolloverWorkflowAsync both use ExecuteDeleteAsync,
// which the EF InMemory provider does not support at all - tests using it would throw at runtime.
public class RolloverRepositoryTests
{
    private readonly IFixture _fixture;

    public RolloverRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    private static ApplicationDbContext CreateDb(string name)
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

    // ------------------------------------------------------------
    // UNAFFECTED - underlying method behaviour did not change in this merge
    // ------------------------------------------------------------

    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsEmptyResult_When_NoRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_NoRecords_" + Guid.NewGuid())
            .Options;

        await using var db = new ApplicationDbContext(options);
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverWorkflowCandidatesCountAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsEmpty_When_NoRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_NoRecords_" + Guid.NewGuid())
            .Options;

        await using var db = new ApplicationDbContext(options);
        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_ReturnsEmpty_When_NoRecords()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_ReturnsEmpty_When_NoRecords));
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverWorkflowCandidatesByRunId(Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_FiltersByRunId_And_IncludedInP1Export()
    {
        var runId = Guid.NewGuid();
        var otherRunId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_FiltersByRunId_And_IncludedInP1Export));

        // Ofqual side - this is the row expected to come back (IncludedInP1Export = true)
        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "QAN-123" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "Org" };
        var qualVersion = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualification.Id,
            Qualification = qualification,
            Organisation = organisation,
            EqfLevel = "Efq",
            Level = "Level",
            Ssa = "ssa",
            Status = "Status",
            SubLevel = "Sublevel",
            Type = "Type"
        };
        var fundingOffer = new FundingOffer { Id = Guid.NewGuid(), Name = "Funding A", DisplayName = "Funding A" };

        var ofqualCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual, qualVersion.Id, fundingOffer.Id, "2024/25", now);
        ofqualCandidate.FundingOffer = fundingOffer;

        var included = RolloverWorkflowCandidate.Create(
            runId, ofqualCandidate.Id, RolloverSourceTypes.Ofqual, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        included.RolloverCandidates = ofqualCandidate;

        // QAA side - this is the row expected to be filtered out (IncludedInP1Export = false).
        // Using a different source type (rather than just a different RolloverRound) also
        // proves the filter and mapping both work correctly across source types, not just
        // that duplicate-key rows are avoided.
        var qaaQualification = RegulatedQaaQualification.Create(
            now, "QAA-999", "QAA Excluded Qualification", "QAA Body",
            DateOnly.FromDateTime(now.AddYears(-1)), DateOnly.FromDateTime(now.AddYears(5)),
            SectorSubjectArea.Science);

        var qaaCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Qaa, qaaQualification.Id, fundingOffer.Id, "2024/25", now);
        qaaCandidate.FundingOffer = fundingOffer;

        var excluded = RolloverWorkflowCandidate.Create(
            runId, qaaCandidate.Id, RolloverSourceTypes.Qaa, qaaQualification.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        typeof(RolloverWorkflowCandidate)
            .GetProperty("IncludedInP1Export")!
            .SetValue(excluded, false);
        excluded.RolloverCandidates = qaaCandidate;

        var differentRun = RolloverWorkflowCandidate.Create(
            otherRunId, ofqualCandidate.Id, RolloverSourceTypes.Ofqual, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        differentRun.RolloverCandidates = ofqualCandidate;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(qualVersion, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.RegulatedQaaQualifications.AddAsync(qaaQualification, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(fundingOffer, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddRangeAsync(ofqualCandidate, qaaCandidate);
        await db.RolloverWorkflowCandidates.AddRangeAsync(included, excluded, differentRun);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverWorkflowCandidatesByRunId(runId, TestContext.Current.CancellationToken)).ToList();
        var row = Assert.Single(result);
        Assert.Equal("QAN-123", row.QAN);
        Assert.Equal("Funding A", row.FundingStreamName);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_MapsAllFieldsCorrectly()
    {
        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_MapsAllFieldsCorrectly));

        var runId = Guid.NewGuid();

        var qualification = new Qualification
        {
            Id = Guid.NewGuid(),
            Qan = "QAN-999",
            QualificationName = "Test Qualification"
        };

        var organisation = new AwardingOrganisation
        {
            Id = Guid.NewGuid(),
            NameOfqual = "Ofqual Org"
        };

        var qualVersion = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualification.Id,
            Qualification = qualification,
            Organisation = organisation,
            Level = "L3",
            Type = "Diploma",
            Ssa = "SSA1",
            OperationalEndDate = new DateTime(2026, 05, 20),
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = false,
            Glh = 120,
            Tqt = 300,
            PreSixteen = true,
            SixteenToEighteen = false,
            EighteenPlus = true,
            NineteenPlus = false,
            EqfLevel = "Efq",
            Status = "Status",
            SubLevel = "Sublevel",
        };

        var fundingOffer = new FundingOffer
        {
            Id = Guid.NewGuid(),
            Name = "Funding A",
            DisplayName = "Funding A"
        };

        var funding = new QualificationFundings
        {
            QualificationVersionId = qualVersion.Id,
            FundingOfferId = fundingOffer.Id,
            StartDate = new DateOnly(2026, 06, 01)
        };

        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual, qualVersion.Id, fundingOffer.Id, "2024/25", DateTime.UtcNow);
        candidate.FundingOffer = fundingOffer;

        var workflowCandidate = RolloverWorkflowCandidate.Create(
            runId, candidate.Id, RolloverSourceTypes.Ofqual, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, DateTime.UtcNow, null, DateTime.UtcNow);
        workflowCandidate.RolloverCandidates = candidate;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(qualVersion, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(fundingOffer, TestContext.Current.CancellationToken);
        await db.QualificationFundings.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddAsync(candidate, TestContext.Current.CancellationToken);
        await db.RolloverWorkflowCandidates.AddAsync(workflowCandidate, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverWorkflowCandidatesByRunId(runId, TestContext.Current.CancellationToken)).ToList();
        var row = Assert.Single(result);

        Assert.Equal("QAN-999", row.QAN);
        Assert.Equal("Test Qualification", row.QualificationTitle);
        Assert.Equal("Ofqual Org", row.AwardingOrganisation);
        Assert.Equal("L3", row.QualificationLevel);
        Assert.Equal("Diploma", row.QualificationType);
        Assert.Equal("SSA1", row.SSA);
        Assert.Equal(new DateTime(2026, 05, 20), row.OperationalEndDate);

        Assert.True(row.OfferedInEngland);
        Assert.False(row.FundedInEngland);

        Assert.Equal(120, row.GLH);
        Assert.Equal(300, row.TQT);

        Assert.True(row.Pre16);
        Assert.False(row.Age16To18);
        Assert.True(row.Age18Plus);
        Assert.False(row.Age19Plus);

        Assert.Equal("Funding A", row.FundingStreamName);
        Assert.Equal(new DateOnly(2026, 06, 01), row.FundingApprovalStartDate);

        Assert.Equal(RolloverStatus.Excluded.ToString(), row.ProposedOutcome); // PassP1 default = false
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_OrdersByQAN()
    {
        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_OrdersByQAN));

        var runId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var qualificationA = new Qualification { Id = Guid.NewGuid(), Qan = "A111" };
        var qualificationB = new Qualification { Id = Guid.NewGuid(), Qan = "B222" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "Org" };

        var versionA = new QualificationVersions { Id = Guid.NewGuid(), Qualification = qualificationA, Organisation = organisation, EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status", SubLevel = "Sublevel", Type = "Type" };
        var versionB = new QualificationVersions { Id = Guid.NewGuid(), Qualification = qualificationB, Organisation = organisation, EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status", SubLevel = "Sublevel", Type = "Type" };

        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "Funding", DisplayName = "Funding" };

        var candidateA = RolloverCandidates.CreateInitialRound(RolloverSourceTypes.Ofqual, versionA.Id, funding.Id, "2024/25", now);
        candidateA.FundingOffer = funding;

        var candidateB = RolloverCandidates.CreateInitialRound(RolloverSourceTypes.Ofqual, versionB.Id, funding.Id, "2024/25", now);
        candidateB.FundingOffer = funding;

        var wcB = RolloverWorkflowCandidate.Create(runId, candidateB.Id, RolloverSourceTypes.Ofqual, versionB.Id, funding.Id, "2024/25", 1, now, null, now);
        wcB.RolloverCandidates = candidateB;

        var wcA = RolloverWorkflowCandidate.Create(runId, candidateA.Id, RolloverSourceTypes.Ofqual, versionA.Id, funding.Id, "2024/25", 1, now, null, now);
        wcA.RolloverCandidates = candidateA;

        await db.Qualification.AddRangeAsync(new[] { qualificationA, qualificationB });
        await db.QualificationVersions.AddRangeAsync(new[] { versionA, versionB });
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddRangeAsync(new[] { candidateA, candidateB });
        await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { wcB, wcA }, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverWorkflowCandidatesByRunId(runId, TestContext.Current.CancellationToken)).ToList();
        Assert.Equal("A111", result[0].QAN);
        Assert.Equal("B222", result[1].QAN);
    }

    [Fact]
    public async Task UpdateRolloverWorkflowCandidatesAsync_DoesNothing_When_EmptyList()
    {
        // Arrange
        await using var db = CreateDb(nameof(UpdateRolloverWorkflowCandidatesAsync_DoesNothing_When_EmptyList));
        var sut = new RolloverRepository(db);

        // Act
        await sut.UpdateRolloverWorkflowCandidatesAsync(new List<RolloverWorkflowCandidate>(), TestContext.Current.CancellationToken);

        // Assert - should not throw and nothing saved
        var count = await db.RolloverWorkflowCandidates.CountAsync(TestContext.Current.CancellationToken);
        count.ShouldBe(0);
    }

    [Fact]
    public async Task UpdateRolloverWorkflowCandidatesAsync_UpdatesAndSaves_When_ListProvided()
    {
        // Arrange
        await using var db = CreateDb(nameof(UpdateRolloverWorkflowCandidatesAsync_UpdatesAndSaves_When_ListProvided));

        var now = DateTime.UtcNow;
        var entity = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Qaa, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        await db.RolloverWorkflowCandidates.AddAsync(entity, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // modify the entity via its public API to change state
        var checks = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = entity.Id,
            SourceType = entity.SourceType,
            SourceQualificationId = entity.SourceQualificationId,
            FundingOfferId = entity.FundingOfferId,
            AcademicYear = entity.AcademicYear,
            IncludedInP1Export = true,
            IncludedInFinalUpload = false,
            CurrentFundingEndDate = now,
            ProposedFundingEndDate = now.AddYears(1),
            FundingStream = "FS",
            RolloverRound = entity.RolloverRound,
            FundingEndDateThreshold = now.AddDays(-1),
            LatestFundingApprovalEndDate = now.AddDays(10),
            OperationalStartDate = now.AddYears(-1),
            OperationalEndDate = now.AddYears(1),
            OperationalEndDateThreshold = now.AddDays(-5),
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true,
            IsOnDefundingList = false
        };

        // Process checks to change PassP1 to true
        entity.ProcessP1Checks(checks);

        var sut = new RolloverRepository(db);

        // Act
        await sut.UpdateRolloverWorkflowCandidatesAsync(new[] { entity }, TestContext.Current.CancellationToken);

        // Assert
        var fetched = await db.RolloverWorkflowCandidates.FirstAsync(TestContext.Current.CancellationToken);
        fetched.PassP1.ShouldBeTrue();
    }

    [Fact]
    public async Task SaveChangesAsync_ProxiesToContextAndPersists()
    {
        // Arrange
        await using var db = CreateDb(nameof(SaveChangesAsync_ProxiesToContextAndPersists));
        var sut = new RolloverRepository(db);

        var run = RolloverWorkflowRun.Create(
            "2024/25",
            SFA.DAS.AODP.Data.Entities.Rollover.Enums.SelectionMethod.FileUpload,
            null,
            null,
            null,
            "user",
            DateTime.UtcNow);

        db.RolloverWorkflowRuns.Add(run);

        // Act
        await sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var persisted = await db.RolloverWorkflowRuns.FirstOrDefaultAsync(r => r.Id == run.Id, TestContext.Current.CancellationToken);
        persisted.ShouldNotBeNull();
    }

    [Fact]
    public async Task GeRolloverWorkflowRunByIdAsync_ReturnsNull_When_NotFound()
    {
        // Arrange
        await using var db = CreateDb(nameof(GeRolloverWorkflowRunByIdAsync_ReturnsNull_When_NotFound));
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GeRolloverWorkflowRunByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GeRolloverWorkflowRunByIdAsync_ReturnsEntity_When_Found()
    {
        // Arrange
        await using var db = CreateDb(nameof(GeRolloverWorkflowRunByIdAsync_ReturnsEntity_When_Found));
        var run = RolloverWorkflowRun.Create(
            "2024/25",
            SFA.DAS.AODP.Data.Entities.Rollover.Enums.SelectionMethod.FileUpload,
            null,
            null,
            null,
            "user",
            DateTime.UtcNow);
        db.RolloverWorkflowRuns.Add(run);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GeRolloverWorkflowRunByIdAsync(run.Id, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(run.Id);
        result.AcademicYear.ShouldBe("2024/25");
    }

    [Fact]
    public async Task GetFundingExtensionValidationContextAsync_Throws_When_NoRuns()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetFundingExtensionValidationContextAsync_Throws_When_NoRuns));
        var sut = new RolloverRepository(db);

        var incoming = new HashSet<CandidateKey> { new CandidateKey("Q-1", "FundX") };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await sut.GetFundingExtensionValidationContextAsync(incoming, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task LoadRolloverCandidateGraphAsync_LoadsNavigationProperties()
    {
        // Arrange
        await using var db = CreateDb(nameof(LoadRolloverCandidateGraphAsync_LoadsNavigationProperties));

        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "Q-GRAPH" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "OrgG" };
        var version = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualification.Id,
            Qualification = qualification,
            Organisation = organisation,
            AwardingOrganisationId = organisation.Id,
            // Fill required non-nullable properties so EF can save the entity
            Status = "Active",
            Type = "TypeA",
            Ssa = "SSA",
            Level = "1",
            SubLevel = "A",
            EqfLevel = "E1"
        };
        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "FundG", DisplayName = "Funding G" };

        var candidate = RolloverCandidates.CreateInitialRound(RolloverSourceTypes.Ofqual, version.Id, funding.Id, "2024/25", DateTime.UtcNow);
        candidate.FundingOffer = funding;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(version, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddAsync(candidate, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // LoadRolloverCandidateGraphAsync only returns a candidate if it is linked to a
        // non-invalidated RolloverWorkflowCandidate row belonging to the latest
        // RolloverWorkflowRun - a bare RolloverCandidates row on its own is not enough.
        var now = DateTime.UtcNow;
        var workflowRun = RolloverWorkflowRun.Create(
            "2024/25", SelectionMethod.QueryBuilder, now, now, now.AddYears(1), "test.user", now);
        await db.RolloverWorkflowRuns.AddAsync(workflowRun, TestContext.Current.CancellationToken);

        var workflowCandidate = RolloverWorkflowCandidate.Create(
            workflowRun.Id, candidate.Id, RolloverSourceTypes.Ofqual, version.Id, funding.Id,
            "2024/25", 1, now, null, now);
        await db.RolloverWorkflowCandidates.AddAsync(workflowCandidate, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var keys = new List<CandidateKey> { new CandidateKey("Q-GRAPH", "FundG") };

        // Act
        var list = await sut.LoadRolloverCandidateGraphAsync(keys, TestContext.Current.CancellationToken);

        // Assert - QualificationVersion is no longer a navigation property on the polymorphic
        // RolloverCandidates entity; LoadRolloverCandidateGraphAsync instead populates
        // SourceQualificationReference via SetSourceContext, sourced from the DB join.
        list.ShouldNotBeNull();
        var fetched = list.Single();
        fetched.FundingOffer.ShouldNotBeNull();
        fetched.SourceQualificationReference.ShouldBe("Q-GRAPH");
        fetched.FundingOffer.Name.ShouldBe("FundG");
    }

    [Fact]
    public async Task DeleteAllWorkflowCandidatesAsync_RemovesAllRecords()
    {
        // Arrange
        await using var db = CreateDb(nameof(DeleteAllWorkflowCandidatesAsync_RemovesAllRecords));

        var now = DateTime.UtcNow;
        var e1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Qaa, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        var e2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Ofqual, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);

        await db.RolloverWorkflowCandidates.AddRangeAsync(e1, e2);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        await sut.DeleteAllWorkflowCandidatesAsync(TestContext.Current.CancellationToken);

        // Assert
        var count = await db.RolloverWorkflowCandidates.CountAsync(TestContext.Current.CancellationToken);
        count.ShouldBe(0);
    }

    [Fact]
    public async Task GetLatestWorkflowRunIdAsync_ReturnsEmpty_When_NoRuns()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetLatestWorkflowRunIdAsync_ReturnsEmpty_When_NoRuns));
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetLatestWorkflowRunIdAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(Guid.Empty);
    }

    [Fact]
    public async Task GetLatestWorkflowRunIdAsync_ReturnsLatestId()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetLatestWorkflowRunIdAsync_ReturnsLatestId));
        var older = RolloverWorkflowRun.Create("2023/24", SelectionMethod.FileUpload, DateTime.UtcNow.AddDays(-2), null, null, "u", DateTime.UtcNow.AddDays(-2));
        var newer = RolloverWorkflowRun.Create("2024/25", SelectionMethod.FileUpload, DateTime.UtcNow, null, null, "u2", DateTime.UtcNow);

        db.RolloverWorkflowRuns.Add(older);
        db.RolloverWorkflowRuns.Add(newer);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetLatestWorkflowRunIdAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(newer.Id);
    }

    [Fact]
    public async Task GetRolloverCandidatesAsync_ReturnsMixedSources_WhenSourceQualificationIdsOverlap()
    {
        await using var db = CreateDb(nameof(GetRolloverCandidatesAsync_ReturnsMixedSources_WhenSourceQualificationIdsOverlap));
        var seed = await SeedMixedSourcesWithSharedSourceIdAsync(db);
        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x =>
            x.SourceType == RolloverSourceTypes.Ofqual &&
            x.SourceQualificationId == seed.SharedSourceQualificationId &&
            x.QualificationNumber == "OFQ-123");
        Assert.Contains(result, x =>
            x.SourceType == RolloverSourceTypes.Qaa &&
            x.SourceQualificationId == seed.SharedSourceQualificationId &&
            x.QualificationNumber == "QAA-123");
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_ReturnsMixedSourceExportRows()
    {
        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_ReturnsMixedSourceExportRows));
        var seed = await SeedMixedSourcesWithSharedSourceIdAsync(db);
        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverWorkflowCandidatesByRunId(seed.WorkflowRunId, TestContext.Current.CancellationToken)).ToList();

        Assert.Equal(2, result.Count);

        var ofqual = result.Single(x => x.QAN == "OFQ-123");
        Assert.Equal("Ofqual Qualification", ofqual.QualificationTitle);
        Assert.Equal("Ofqual Org", ofqual.AwardingOrganisation);
        Assert.Equal(new DateOnly(2026, 8, 1), ofqual.FundingApprovalStartDate);

        var qaa = result.Single(x => x.QAN == "QAA-123");
        Assert.Equal("QAA Qualification", qaa.QualificationTitle);
        Assert.Equal("QAA Awarding Body", qaa.AwardingOrganisation);
        Assert.Equal(new DateOnly(2026, 9, 1), qaa.FundingApprovalStartDate);
        Assert.True(qaa.OfferedInEngland);
        Assert.True(qaa.FundedInEngland);
        Assert.True(qaa.Age16To18);
        Assert.True(qaa.Age18Plus);
    }

    [Fact]
    public async Task GetFundingExtensionValidationContextAsync_MatchesMixedSourceCandidateKeys()
    {
        await using var db = CreateDb(nameof(GetFundingExtensionValidationContextAsync_MatchesMixedSourceCandidateKeys));
        await SeedMixedSourcesWithSharedSourceIdAsync(db);
        var sut = new RolloverRepository(db);

        var incoming = new HashSet<CandidateKey>
            {
                new("OFQ-123", "Funding A"),
                new("QAA-123", "Funding A")
            };

        var result = await sut.GetFundingExtensionValidationContextAsync(incoming, TestContext.Current.CancellationToken);

        Assert.Equal(2, result.CandidatesInDb.Count);
        Assert.Contains(new CandidateKey("OFQ-123", "Funding A"), result.CandidatesInDb);
        Assert.Contains(new CandidateKey("QAA-123", "Funding A"), result.CandidatesInDb);
        Assert.Equal(2, result.WorkflowCandidatesInDb.Count);
        Assert.Contains(new CandidateKey("OFQ-123", "Funding A"), result.WorkflowCandidatesInDb);
        Assert.Contains(new CandidateKey("QAA-123", "Funding A"), result.WorkflowCandidatesInDb);
    }

    [Fact]
    public async Task LoadRolloverCandidateGraphAsync_SetsSourceContextForMixedSources()
    {
        await using var db = CreateDb(nameof(LoadRolloverCandidateGraphAsync_SetsSourceContextForMixedSources));
        var seed = await SeedMixedSourcesWithSharedSourceIdAsync(db);
        var sut = new RolloverRepository(db);

        var result = await sut.LoadRolloverCandidateGraphAsync(
            [
                new CandidateKey("OFQ-123", "Funding A"),
                    new CandidateKey("QAA-123", "Funding A")
            ],
            TestContext.Current.CancellationToken);

        Assert.Equal(2, result.Count);

        var ofqual = result.Single(x => x.SourceType == RolloverSourceTypes.Ofqual);
        Assert.Equal("OFQ-123", ofqual.SourceQualificationReference);
        Assert.Equal(seed.OfqualQualificationId, ofqual.DiscussionQualificationId);

        var qaa = result.Single(x => x.SourceType == RolloverSourceTypes.Qaa);
        Assert.Equal("QAA-123", qaa.SourceQualificationReference);
        Assert.Null(qaa.DiscussionQualificationId);
    }

    [Fact]
    public async Task LoadRolloverCandidateGraphAsync_ExcludesInvalidatedWorkflowCandidate()
    {
        await using var db = CreateDb(
            nameof(LoadRolloverCandidateGraphAsync_ExcludesInvalidatedWorkflowCandidate));
        await SeedMixedSourcesWithSharedSourceIdAsync(db);
        var workflowCandidates = await db.RolloverWorkflowCandidates.ToListAsync(
            TestContext.Current.CancellationToken);
        foreach (var workflowCandidate in workflowCandidates)
        {
            workflowCandidate.Invalidate(
                "Funding changed.",
                new DateTime(2026, 7, 2, 10, 0, 0));
        }
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sut = new RolloverRepository(db);

        var result = await sut.LoadRolloverCandidateGraphAsync(
            [
                new CandidateKey("OFQ-123", "Funding A"),
                    new CandidateKey("QAA-123", "Funding A")
            ],
            TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRolloverCandidatesStatusAsync_ReturnsMixedSourceStatuses()
    {
        await using var db = CreateDb(nameof(GetRolloverCandidatesStatusAsync_ReturnsMixedSourceStatuses));
        await SeedMixedSourcesWithSharedSourceIdAsync(db);
        var sut = new RolloverRepository(db);

        var result = await sut.GetRolloverCandidatesStatusAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Qan == "OFQ-123" && x.FundingStreamName == "Funding A");
        Assert.Contains(result, x => x.Qan == "QAA-123" && x.FundingStreamName == "Funding A");
    }

    private static async Task<MixedSourceSeed> SeedMixedSourcesWithSharedSourceIdAsync(ApplicationDbContext db)
    {
        var now = new DateTime(2026, 7, 1, 10, 0, 0);
        var sharedSourceQualificationId = Guid.NewGuid();
        var ofqualQualificationId = Guid.NewGuid();

        var fundingOffer = new FundingOffer
        {
            Id = Guid.NewGuid(),
            Name = "Funding A",
            DisplayName = "Funding A"
        };

        var qualification = new Qualification
        {
            Id = ofqualQualificationId,
            Qan = "OFQ-123",
            QualificationName = "Ofqual Qualification"
        };

        var organisation = new AwardingOrganisation
        {
            Id = Guid.NewGuid(),
            NameOfqual = "Ofqual Org"
        };

        var qualificationVersion = new QualificationVersions
        {
            Id = sharedSourceQualificationId,
            QualificationId = qualification.Id,
            Qualification = qualification,
            Organisation = organisation,
            Level = "L3",
            Type = "Diploma",
            Ssa = "SSA",
            EqfLevel = "EQF",
            Status = "Approved",
            SubLevel = "Sub",
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true
        };

        var qaaQualification = RegulatedQaaQualification.Create(
            now,
            "QAA-123",
            "QAA Qualification",
            "QAA Awarding Body",
            new DateOnly(2026, 9, 1),
            new DateOnly(2027, 9, 1),
            SectorSubjectArea.AccountingAndFinance);

        SetPrivate(qaaQualification, nameof(RegulatedQaaQualification.Id), sharedSourceQualificationId);

        var ofqualCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual,
            qualificationVersion.Id,
            fundingOffer.Id,
            "2026/27",
            now);
        ofqualCandidate.FundingOffer = fundingOffer;

        var qaaCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Qaa,
            qaaQualification.Id,
            fundingOffer.Id,
            "2026/27",
            now);
        qaaCandidate.FundingOffer = fundingOffer;

        var ofqualFunding = new QualificationFundings
        {
            Id = Guid.NewGuid(),
            QualificationVersionId = qualificationVersion.Id,
            FundingOfferId = fundingOffer.Id,
            StartDate = new DateOnly(2026, 8, 1)
        };

        var qaaFunding = QaaQualificationFunding.Create(
            qaaQualification.Id,
            fundingOffer.Id,
            new DateOnly(2026, 9, 1),
            new DateOnly(2027, 9, 1),
            "Approved",
            now);

        var workflowRun = RolloverWorkflowRun.Create(
            "2026/27",
            SelectionMethod.QueryBuilder,
            now,
            now,
            now.AddYears(1),
            "test",
            now);

        await db.FundingOffers.AddAsync(fundingOffer, TestContext.Current.CancellationToken);
        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(qualificationVersion, TestContext.Current.CancellationToken);
        await db.RegulatedQaaQualifications.AddAsync(qaaQualification, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddRangeAsync(ofqualCandidate, qaaCandidate);
        await db.QualificationFundings.AddAsync(ofqualFunding, TestContext.Current.CancellationToken);
        await db.QaaQualificationFundings.AddAsync(qaaFunding, TestContext.Current.CancellationToken);
        await db.RolloverWorkflowRuns.AddAsync(workflowRun, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var ofqualWorkflowCandidate = RolloverWorkflowCandidate.Create(
            workflowRun.Id,
            ofqualCandidate.Id,
            RolloverSourceTypes.Ofqual,
            qualificationVersion.Id,
            fundingOffer.Id,
            "2026/27",
            1,
            now,
            now.AddYears(1),
            now);
        ofqualWorkflowCandidate.RolloverCandidates = ofqualCandidate;

        var qaaWorkflowCandidate = RolloverWorkflowCandidate.Create(
            workflowRun.Id,
            qaaCandidate.Id,
            RolloverSourceTypes.Qaa,
            qaaQualification.Id,
            fundingOffer.Id,
            "2026/27",
            1,
            now,
            now.AddYears(1),
            now);
        qaaWorkflowCandidate.RolloverCandidates = qaaCandidate;

        await db.RolloverWorkflowCandidates.AddRangeAsync(ofqualWorkflowCandidate, qaaWorkflowCandidate);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        return new MixedSourceSeed(
            sharedSourceQualificationId,
            ofqualQualificationId,
            workflowRun.Id);
    }

    private static void SetPrivate<T>(object instance, string propertyName, T value)
    {
        instance
            .GetType()
            .GetProperty(propertyName)!
            .SetValue(instance, value);
    }

    private record MixedSourceSeed(
        Guid SharedSourceQualificationId,
        Guid OfqualQualificationId,
        Guid WorkflowRunId);

    [Fact]
    public async Task GetRolloverWorkflowCandidatesCountAsync_ReturnsRecordCount()
    {
        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesCountAsync_ReturnsRecordCount));

        var now = DateTime.UtcNow;
        var run = RolloverWorkflowRun.Create("2024/25", SelectionMethod.QueryBuilder, now, now, now.AddYears(1), "test.user", now);
        await db.RolloverWorkflowRuns.AddAsync(run, TestContext.Current.CancellationToken);

        var e1 = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Ofqual, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e2 = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Qaa, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));
        var e3 = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Ofqual, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);

        await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverWorkflowCandidatesCountAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsAllRecords()
    {
        await using var db = CreateDb(nameof(GetAllRolloverWorkflowCandidatesAsync_ReturnsAllRecords));

        var now = DateTime.UtcNow;

        var run = RolloverWorkflowRun.Create("2024/25", SelectionMethod.QueryBuilder, now, now, now.AddYears(1), "test.user", now);
        await db.RolloverWorkflowRuns.AddAsync(run, TestContext.Current.CancellationToken);

        var e1 = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Qaa, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-3), null, now.AddDays(-3));
        var e2 = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Ofqual, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e3 = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Qaa, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));

        await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, r => r.CreatedAt == e1.CreatedAt);
        Assert.Contains(result, r => r.CreatedAt == e2.CreatedAt);
        Assert.Contains(result, r => r.CreatedAt == e3.CreatedAt);
    }



    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ExcludesInvalidatedCandidates()
    {
        await using var db = CreateDb(nameof(GetAllRolloverWorkflowCandidatesAsync_ExcludesInvalidatedCandidates));

        var now = DateTime.UtcNow;

        var run = RolloverWorkflowRun.Create("2024/25", SelectionMethod.QueryBuilder, now, now, now.AddYears(1), "test.user", now);
        await db.RolloverWorkflowRuns.AddAsync(run, TestContext.Current.CancellationToken);

        var active = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Qaa, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        var invalidated = RolloverWorkflowCandidate.Create(run.Id, Guid.NewGuid(), RolloverSourceTypes.Ofqual,  Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        invalidated.Invalidate("test invalidation", now);

        await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { active, invalidated });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

        var row = Assert.Single(result);
        Assert.Equal(active.Id, row.Id);
    }

    [Fact]
    public async Task GetRolloverCandidatesAsync_ReturnsOnlyActive_With_ProperProjection()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverCandidatesAsync_ReturnsOnlyActive_With_ProperProjection));

        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "Q-1" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "Org" };
        var qualVersion = new QualificationVersions { Id = Guid.NewGuid(), QualificationId = qualification.Id, Qualification = qualification, Organisation = organisation, EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status", SubLevel = "Sublevel", Type = "Type" };
        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "FundX", DisplayName = "Fund X" };

        var active = RolloverCandidates.CreateInitialRound(RolloverSourceTypes.Ofqual, qualVersion.Id, funding.Id, "2024/25", DateTime.UtcNow);
        // CreateInitialRound sets IsActive = true so no setter needed
        active.FundingOffer = funding;

        var inactive = RolloverCandidates.CreateInitialRound(RolloverSourceTypes.Qaa, qualVersion.Id, funding.Id, "2024/25", DateTime.UtcNow);
        inactive.FundingOffer = funding;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(qualVersion, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddRangeAsync(new[] { active, inactive }, TestContext.Current.CancellationToken);

        // set inactive flag using EF property API
        db.Entry(inactive).Property("IsActive").CurrentValue = false;

        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetRolloverCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

        // Assert
        result.Count.ShouldBe(1);
        var dto = result.Single();
        dto.Id.ShouldBe(active.Id);
        dto.SourceType.ShouldBe(RolloverSourceTypes.Ofqual);
        dto.SourceQualificationId.ShouldBe(active.SourceQualificationId);
        dto.FundingOfferId.ShouldBe(active.FundingOfferId);
        dto.FundingOfferName.ShouldBe("FundX");
        dto.QualificationNumber.ShouldBe("Q-1");
        dto.AcademicYear.ShouldBe(active.AcademicYear);
    }

    [Fact]
    public async Task GetRolloverCandidatesStatusAsync_ReturnsProjectedItems()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverCandidatesStatusAsync_ReturnsProjectedItems));

        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "Q-STAT" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "OrgStat" };
        var version = new QualificationVersions { Id = Guid.NewGuid(), QualificationId = qualification.Id, Qualification = qualification, Organisation = organisation, Status = "Approved", Type = "Standard", Ssa = "001", Level = "3", SubLevel = "A", EqfLevel = "EQF3" };
        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "FundStat", DisplayName = "Fund Stat" };

        var candidate = RolloverCandidates.CreateInitialRound(RolloverSourceTypes.Ofqual, version.Id, funding.Id, "2024/25", DateTime.UtcNow);
        candidate.FundingOffer = funding;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(version, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddAsync(candidate, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverCandidatesStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        var item = result.SingleOrDefault(x => x.Qan == "Q-STAT" && x.FundingStreamName == "FundStat");
        item.ShouldNotBeNull();
        item!.RolloverStatus.ShouldBe(candidate.RolloverStatus);
    }
}
