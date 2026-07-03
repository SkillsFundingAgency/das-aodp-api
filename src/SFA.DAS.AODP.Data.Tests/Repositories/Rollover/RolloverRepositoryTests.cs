using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverRepositoryTests
{
    private readonly IFixture _fixture;

    public RolloverRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    private static ApplicationDbContext CreateDb(string name)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"{name}_{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        return new ApplicationDbContext(options);
    }


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
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsPagedOrderedMappedData()
    {
        // Arrange 
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_Paged_" + Guid.NewGuid())
            .Options;

        var now = DateTime.UtcNow;
        var e1 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e2 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));
        var e3 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act 
            var result = await sut.GetRolloverWorkflowCandidatesCountAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(3, result);

        }
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
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsAllRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_AllRecords_" + Guid.NewGuid())
            .Options;

        var now = DateTime.UtcNow;
        var e1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-3), null, now.AddDays(-3));
        var e2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e3 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act
            var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.CreatedAt == e1.CreatedAt);
            Assert.Contains(result, r => r.CreatedAt == e2.CreatedAt);
            Assert.Contains(result, r => r.CreatedAt == e3.CreatedAt);
        }
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesP1ChecksAsync_ReturnsEmpty_When_NoRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("RolloverP1_NoRecords_" + Guid.NewGuid())
            .Options;

        await using var db = new ApplicationDbContext(options);
        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetRolloverWorkflowCandidatesP1ChecksAsync(TestContext.Current.CancellationToken)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesP1ChecksAsync_ReturnsAllRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("RolloverP1_AllRecords_" + Guid.NewGuid())
            .Options;

        var now = DateTime.UtcNow;
        var e1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = Guid.NewGuid(),
            QualificationVersionId = Guid.NewGuid(),
            FundingOfferId = Guid.NewGuid(),
            AcademicYear = "2025/26",
            IncludedInP1Export = true,
            IncludedInFinalUpload = false,
            CurrentFundingEndDate = now.Date,
            ProposedFundingEndDate = now.Date.AddYears(1),
            FundingStream = "FS1",
            RolloverRound = 1,
            FundingEndDateThreshold = now.Date.AddDays(-10),
            LatestFundingApprovalEndDate = now.Date.AddDays(-20),
            OperationalStartDate = now.Date.AddYears(-1),
            OperationalEndDate = now.Date.AddMonths(6),
            OperationalEndDateThreshold = now.Date.AddDays(-5),
            OfferedInEngland = true,
            IsOnDefundingList = false
        };

        var e2 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = Guid.NewGuid(),
            QualificationVersionId = Guid.NewGuid(),
            FundingOfferId = Guid.NewGuid(),
            AcademicYear = "2026/27",
            IncludedInP1Export = false,
            IncludedInFinalUpload = true,
            CurrentFundingEndDate = now.Date,
            ProposedFundingEndDate = null,
            FundingStream = null,
            RolloverRound = 2,
            FundingEndDateThreshold = now.Date.AddDays(30),
            LatestFundingApprovalEndDate = now.Date.AddDays(60),
            OperationalStartDate = now.Date,
            OperationalEndDate = null,
            OperationalEndDateThreshold = now.Date.AddDays(15),
            OfferedInEngland = false,
            IsOnDefundingList = true
        };

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidatesP1Checks.AddRangeAsync(new[] { e1, e2 });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act
            var result = (await sut.GetRolloverWorkflowCandidatesP1ChecksAsync(TestContext.Current.CancellationToken)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.WorkflowCandidateId == e1.WorkflowCandidateId);
            Assert.Contains(result, r => r.WorkflowCandidateId == e2.WorkflowCandidateId);

            var fetched1 = result.Single(r => r.WorkflowCandidateId == e1.WorkflowCandidateId);
            Assert.Equal(e1.AcademicYear, fetched1.AcademicYear);
            Assert.Equal(e1.FundingStream, fetched1.FundingStream);
            Assert.Equal(e1.IsOnDefundingList, fetched1.IsOnDefundingList);

            var fetched2 = result.Single(r => r.WorkflowCandidateId == e2.WorkflowCandidateId);
            Assert.Equal(e2.AcademicYear, fetched2.AcademicYear);
            Assert.Equal(e2.FundingStream, fetched2.FundingStream);
            Assert.Equal(e2.IsOnDefundingList, fetched2.IsOnDefundingList);
        }
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

        // Minimal qualification + version + funding offer so projection works
        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "QAN-123" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "Org" };
        var qualVersion = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualification.Id,
            Qualification = qualification,
            Organisation = organisation,
            EqfLevel = "Efq",
            Level ="Level",
            Ssa ="ssa",
            Status ="Status",
            SubLevel = "Sublevel",
            Type = "Type"
        };
        var fundingOffer = new FundingOffer { Id = Guid.NewGuid(), Name = "Funding A" };

        var candidate = RolloverCandidates.CreateInitialRound(
            qualVersion.Id, fundingOffer.Id, "2024/25", now);
        candidate.QualificationVersion = qualVersion;
        candidate.FundingOffer = fundingOffer;

        var included = RolloverWorkflowCandidate.Create(
            runId, candidate.Id, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        included.RolloverCandidates = candidate;

        var excluded = RolloverWorkflowCandidate.Create(
            runId, candidate.Id, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        typeof(RolloverWorkflowCandidate)
            .GetProperty("IncludedInP1Export")!
            .SetValue(excluded, false);
        excluded.RolloverCandidates = candidate;

        var differentRun = RolloverWorkflowCandidate.Create(
            otherRunId, candidate.Id, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        differentRun.RolloverCandidates = candidate;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(qualVersion, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(fundingOffer, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddAsync(candidate, TestContext.Current.CancellationToken);
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
            Name = "Funding A"
        };

        var funding = new QualificationFundings
        {
            QualificationVersionId = qualVersion.Id,
            FundingOfferId = fundingOffer.Id,
            StartDate = new DateOnly(2026, 06, 01)
        };

        var candidate = RolloverCandidates.CreateInitialRound(
            qualVersion.Id, fundingOffer.Id, "2024/25", DateTime.UtcNow);
        candidate.QualificationVersion = qualVersion;
        candidate.FundingOffer = fundingOffer;

        var workflowCandidate = RolloverWorkflowCandidate.Create(
            runId, candidate.Id, qualVersion.Id, fundingOffer.Id,
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

        var versionA = new QualificationVersions { Id = Guid.NewGuid(), Qualification = qualificationA, Organisation = organisation,EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status",SubLevel = "Sublevel",Type = "Type"};
        var versionB = new QualificationVersions { Id = Guid.NewGuid(), Qualification = qualificationB, Organisation = organisation, EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status", SubLevel = "Sublevel", Type = "Type" };

        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "Funding" };

        var candidateA = RolloverCandidates.CreateInitialRound(versionA.Id, funding.Id, "2024/25", now);
        candidateA.QualificationVersion = versionA;
        candidateA.FundingOffer = funding;

        var candidateB = RolloverCandidates.CreateInitialRound(versionB.Id, funding.Id, "2024/25", now);
        candidateB.QualificationVersion = versionB;
        candidateB.FundingOffer = funding;

        var wcB = RolloverWorkflowCandidate.Create(runId, candidateB.Id, versionB.Id, funding.Id, "2024/25", 1, now, null, now);
        wcB.RolloverCandidates = candidateB;

        var wcA = RolloverWorkflowCandidate.Create(runId, candidateA.Id, versionA.Id, funding.Id, "2024/25", 1, now, null, now);
        wcA.RolloverCandidates = candidateA;

        await db.Qualification.AddRangeAsync(qualificationA, qualificationB);
        await db.QualificationVersions.AddRangeAsync(versionA, versionB);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddRangeAsync(candidateA, candidateB);
        await db.RolloverWorkflowCandidates.AddRangeAsync(wcB, wcA);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverWorkflowCandidatesByRunId(runId, TestContext.Current.CancellationToken)).ToList();
        Assert.Equal("A111", result[0].QAN);
        Assert.Equal("B222", result[1].QAN);
    }


}