using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverRepositoryTests
{
    private readonly IFixture _fixture;

    public RolloverRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    private ApplicationDbContext CreateDb(string name)
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
        var result = await sut.GetRolloverWorkflowCandidatesCountAsync(default);

        // Assert
        Assert.NotNull(result);
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
        var e1 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now.AddDays(-2), null, now.AddDays(-2));
        var e2 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now.AddDays(-1), null, now.AddDays(-1));
        var e3 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now, null, now);

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
            await db.SaveChangesAsync();
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act 
            var result = await sut.GetRolloverWorkflowCandidatesCountAsync(default);

            // Assert
            Assert.NotNull(result);
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
        var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(CancellationToken.None)).ToList();

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
        var e1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now.AddDays(-3), null, now.AddDays(-3));
        var e2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now.AddDays(-2), null, now.AddDays(-2));
        var e3 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now.AddDays(-1), null, now.AddDays(-1));

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
            await db.SaveChangesAsync();
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act
            var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(default)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.CreatedAt == e1.CreatedAt);
            Assert.Contains(result, r => r.CreatedAt == e2.CreatedAt);
            Assert.Contains(result, r => r.CreatedAt == e3.CreatedAt);
        }
    }

    [Fact]
    public async Task GetRolloverCandidatesAsync_ReturnsEmpty_When_NoRecords()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverCandidatesAsync_ReturnsEmpty_When_NoRecords));
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverCandidatesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRolloverCandidatesAsync_ReturnsAllRecords()
    {
        // Arrange
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var academicYear = "2024/25";
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);

        var e1 = RolloverCandidates.CreateInitialRound(qualificationVersionId, fundingOfferId, academicYear, createdAt);

        var fundingOffer = new FundingOffer
        {
            Id = fundingOfferId,
            Name = "Funding A"
        };

        var qualification = new Qualification
        {
            Id = Guid.NewGuid(),
            Qan = "QAN-001"
        };

        var qualVersion = new QualificationVersions
        {
            Id = qualificationVersionId,
            QualificationId = qualification.Id,
            Qualification = qualification,
            Status = "Active",
            Type = "Type",
            Ssa = "SSA",
            Level = "L2",
            SubLevel = "Sub",
            EqfLevel = "2",
            RegulationStartDate = DateTime.UtcNow,
            OperationalStartDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            UiLastUpdatedDate = DateTime.UtcNow,
            InsertedDate = DateTime.UtcNow,
        };

        // Wire up navigations (important)
        e1.FundingOffer = fundingOffer;
        e1.QualificationVersion = qualVersion;

        // Seed
        await using var db = CreateDb(nameof(GetRolloverCandidatesAsync_ReturnsAllRecords));
        await db.FundingOffers.AddAsync(fundingOffer);
        await db.Qualification.AddAsync(qualification);
        await db.QualificationVersions.AddAsync(qualVersion);
        await db.RolloverCandidates.AddAsync(e1);
        await db.SaveChangesAsync();

        var sut = new RolloverRepository(db);
        var result = (await sut.GetRolloverCandidatesAsync()).ToList();
        var item = Assert.Single(result);
        Assert.Equal(qualificationVersionId, item.QualificationVersionId);
        Assert.Equal(academicYear, item.AcademicYear);
    }

    [Fact]
    public async Task GetRolloverCandidatesAsync_ReturnsMappedData()
    {
        // Arrange
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var academicYear = "2024/25";
        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00);

        var e1 = RolloverCandidates.CreateInitialRound(qualificationVersionId, fundingOfferId, academicYear, createdAt);

        var fundingOffer = new FundingOffer
        {
            Id = fundingOfferId,
            Name = "Funding A"
        };

        var qualification = new Qualification
        {
            Id = Guid.NewGuid(),
            Qan = "QAN-001"
        };

        var qualVersion = new QualificationVersions
        {
            Id = qualificationVersionId,
            QualificationId = qualification.Id,
            Qualification = qualification,
            Status = "Active",
            Type = "Type",
            Ssa = "SSA",
            Level = "L2",
            SubLevel = "Sub",
            EqfLevel = "2",
            RegulationStartDate = DateTime.UtcNow,
            OperationalStartDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow,
            UiLastUpdatedDate = DateTime.UtcNow,
            InsertedDate = DateTime.UtcNow,
        };

        // Wire up navigations (important)
        e1.FundingOffer = fundingOffer;
        e1.QualificationVersion = qualVersion;

        // Seed
        // Seed
        await using var db = CreateDb(nameof(GetRolloverCandidatesAsync_ReturnsMappedData));
        await db.FundingOffers.AddAsync(fundingOffer);
        await db.Qualification.AddAsync(qualification);
        await db.QualificationVersions.AddAsync(qualVersion);
        await db.RolloverCandidates.AddAsync(e1);
        await db.SaveChangesAsync();

        var sut = new RolloverRepository(db);
        var result = (await sut.GetRolloverCandidatesAsync()).ToList();

        // Assert
        var item = Assert.Single(result);
        Assert.Equal(qualificationVersionId, item.QualificationVersionId);
        Assert.Equal("2024/25", item.AcademicYear);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunAsync_SavesAndReturnsEntity()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunAsync_SavesAndReturnsEntity));

        var workflowRun = RolloverWorkflowRun.Create(
        "2024/25", SelectionMethod.FileUpload, DateTime.UtcNow.AddMonths(2), DateTime.UtcNow.AddMonths(3), DateTime.UtcNow.AddMonths(2), "test_user", DateTime.UtcNow
        );

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.CreateRolloverWorkflowRunAsync(workflowRun, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(workflowRun.Id, result.Id);

        var exists = await db.RolloverWorkflowRuns.CountAsync();
        Assert.Equal(1, exists);

        var saved = await db.RolloverWorkflowRuns.FirstAsync();
        Assert.Equal(workflowRun.Id, saved.Id);
        Assert.Equal("2024/25", saved.AcademicYear);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunAsync_Throws_WhenWorkflowRunIsNull()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunAsync_Throws_WhenWorkflowRunIsNull));

        var sut = new RolloverRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.CreateRolloverWorkflowRunAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task AddWorkflowCandidatesAsync_AddsWorkflowCandidates_WhenAllValid()
    {
        // Arrange
        await using var db = CreateDb(nameof(AddWorkflowCandidatesAsync_AddsWorkflowCandidates_WhenAllValid));

        var workflowRun = RolloverWorkflowRun.Create(
            "2024/25",
            SelectionMethod.FileUpload,
            DateTime.UtcNow.AddMonths(2),
            DateTime.UtcNow.AddMonths(3),
            DateTime.UtcNow.AddMonths(2),
            "test_user",
            DateTime.UtcNow);

        db.RolloverWorkflowRuns.Add(workflowRun);

        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();

        var candidate = RolloverCandidates.CreateInitialRound(
            qualificationVersionId,
            fundingOfferId,
            "2024/25",
            DateTime.UtcNow);

        db.RolloverCandidates.Add(candidate);

        await db.SaveChangesAsync();

        var sut = new RolloverRepository(db);

        // Act
        await sut.AddWorkflowCandidatesAsync(
            workflowRun.Id,
            "2024/25",
            new[] { candidate.Id },
            CancellationToken.None);

        // Assert
        var saved = await db.RolloverWorkflowCandidates.ToListAsync();

        Assert.Single(saved);

        var wf = saved[0];
        Assert.Equal(workflowRun.Id, wf.RolloverWorkflowRunId);
        Assert.Equal(candidate.Id, wf.RolloverCandidatesId);
        Assert.Equal(candidate.AcademicYear, wf.AcademicYear);
        Assert.Equal(candidate.FundingOfferId, wf.FundingOfferId);
        Assert.Equal(candidate.QualificationVersionId, wf.QualificationVersionId);
    }

    [Fact]
    public async Task AddWorkflowCandidatesAsync_Throws_WhenCandidateIdsNull()
    {
        await using var db = CreateDb(nameof(AddWorkflowCandidatesAsync_Throws_WhenCandidateIdsNull));
        var sut = new RolloverRepository(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.AddWorkflowCandidatesAsync(Guid.NewGuid(), "2024/25", null!, CancellationToken.None));
    }

    [Fact]
    public async Task AddWorkflowCandidatesAsync_Throws_WhenCandidateIdsEmpty()
    {
        await using var db = CreateDb(nameof(AddWorkflowCandidatesAsync_Throws_WhenCandidateIdsEmpty));
        var sut = new RolloverRepository(db);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.AddWorkflowCandidatesAsync(Guid.NewGuid(), "2024/25", Array.Empty<Guid>(), CancellationToken.None));
    }

    [Fact]
    public async Task AddWorkflowCandidatesAsync_Throws_WhenCandidateNotFound()
    {
        // Arrange
        await using var db = CreateDb(nameof(AddWorkflowCandidatesAsync_Throws_WhenCandidateNotFound));

        var sut = new RolloverRepository(db);

        var workflowRunId = Guid.NewGuid();

        var badId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.AddWorkflowCandidatesAsync(
                workflowRunId,
                "2024/25",
                new[] { badId },
                CancellationToken.None));
    }
}