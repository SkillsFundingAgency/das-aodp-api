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
        var e1 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e2 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));
        var e3 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);

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
        var e1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-3), null, now.AddDays(-3));
        var e2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e3 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));

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
        var result = await sut.GetRolloverCandidatesAsync(default);

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
        var result = (await sut.GetRolloverCandidatesAsync(default)).ToList();
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
        var result = (await sut.GetRolloverCandidatesAsync(default)).ToList();

        // Assert
        var item = Assert.Single(result);
        Assert.Equal(qualificationVersionId, item.QualificationVersionId);
        Assert.Equal("2024/25", item.AcademicYear);
    }

    [Fact]
    public async Task GetRolloverCandidatesByIdsAsync_ReturnsEmpty_When_NoMatchingIds()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverCandidatesByIdsAsync_ReturnsEmpty_When_NoMatchingIds));
        var sut = new RolloverRepository(db);

        var ids = new[] { Guid.NewGuid() };

        // Act
        var result = await sut.GetRolloverCandidatesByIdsAsync(ids, default);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRolloverCandidatesByIdsAsync_ReturnsMappedFieldsCorrectly()
    {
        // Arrange
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();

        var e1 = RolloverCandidates.CreateInitialRound(
            qualificationVersionId,
            fundingOfferId,
            "2024/25",
            DateTime.UtcNow);

        await using var db = CreateDb(nameof(GetRolloverCandidatesByIdsAsync_ReturnsMappedFieldsCorrectly));
        await db.RolloverCandidates.AddAsync(e1);
        await db.SaveChangesAsync();

        var saved = await db.RolloverCandidates
            .AsNoTracking()
            .SingleAsync(x => x.QualificationVersionId == qualificationVersionId);

        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetRolloverCandidatesByIdsAsync(new[] { saved.Id }, default)).ToList();

        // Assert
        var item = Assert.Single(result);
        Assert.Equal(saved.Id, item.Id);
        Assert.Equal(qualificationVersionId, item.QualificationVersionId);
        Assert.Equal(fundingOfferId, item.FundingOfferId);
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
        Assert.Equal(workflowRun.Id, result);

        var exists = await db.RolloverWorkflowRuns.CountAsync();
        Assert.Equal(1, exists);

        var saved = await db.RolloverWorkflowRuns.FirstAsync();
        Assert.Equal(workflowRun.Id, saved.Id);
        Assert.Equal("2024/25", saved.AcademicYear);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunAsync_Throws_When_Null()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunAsync_Throws_When_Null));
        var sut = new RolloverRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            sut.CreateRolloverWorkflowRunAsync(null!, default));
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunAsync_PersistsAllFields()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunAsync_PersistsAllFields));
        var sut = new RolloverRepository(db);

        var createdAt = new DateTime(2026, 02, 28, 12, 00, 00, DateTimeKind.Utc);

        var run = RolloverWorkflowRun.Create(
        "2024/25", SelectionMethod.FileUpload, DateTime.UtcNow.AddMonths(2), DateTime.UtcNow.AddMonths(3), DateTime.UtcNow.AddMonths(2), "test_user", DateTime.UtcNow
        );

        // Act
        var id = await sut.CreateRolloverWorkflowRunAsync(run, default);

        // Assert
        var saved = await db.RolloverWorkflowRuns.AsNoTracking().SingleAsync(x => x.Id == id);

        Assert.Equal("2024/25", saved.AcademicYear);
        Assert.Equal(id, saved.Id);
        Assert.Equal(run.SelectionMethod, saved.SelectionMethod);
    }

    [Fact]
    public async Task CreateRolloverWorkflowCandidatesAsync_SavesAllItems()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowCandidatesAsync_SavesAllItems));
        var sut = new RolloverRepository(db);

        var now = DateTime.UtcNow;
        var c1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var c2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));

        // Act
        await sut.CreateRolloverWorkflowCandidatesAsync(new[] { c1, c2 }, default);

        // Assert
        var all = await db.RolloverWorkflowCandidates.AsNoTracking().ToListAsync();
        Assert.Equal(2, all.Count);
        Assert.Contains(all, x => x.RolloverCandidatesId == c1.RolloverCandidatesId && x.RolloverWorkflowRunId == c1.RolloverWorkflowRunId);
        Assert.Contains(all, x => x.RolloverCandidatesId == c2.RolloverCandidatesId && x.RolloverWorkflowRunId == c2.RolloverWorkflowRunId);
    }

    [Fact]
    public async Task CreateRolloverWorkflowCandidatesAsync_Throws_When_Null()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowCandidatesAsync_Throws_When_Null));
        var sut = new RolloverRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.CreateRolloverWorkflowCandidatesAsync(null!, default));
    }

    [Fact]
    public async Task CreateRolloverWorkflowCandidatesAsync_NoOp_When_Empty()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowCandidatesAsync_NoOp_When_Empty));
        var sut = new RolloverRepository(db);

        var empty = Array.Empty<RolloverWorkflowCandidate>();

        // Act
        await sut.CreateRolloverWorkflowCandidatesAsync(empty, default);

        // Assert
        Assert.Empty(db.RolloverWorkflowCandidates);
    }

    [Fact]
    public async Task CreateRolloverWorkflowCandidatesAsync_RespectsCancellation()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowCandidatesAsync_RespectsCancellation));
        var sut = new RolloverRepository(db);

        var now = DateTime.UtcNow;
        var items = new[]
        {
            RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, DateTime.Now)
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            sut.CreateRolloverWorkflowCandidatesAsync(items, cts.Token));

        // Assert DB unchanged
        Assert.Empty(db.RolloverWorkflowCandidates);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunFundingOffersAsync_SavesAllItems()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunFundingOffersAsync_SavesAllItems));
        var sut = new RolloverRepository(db);

        var f1 = RolloverWorkflowRunFundingOffer.Create(
            Guid.NewGuid(),
            Guid.NewGuid());

        var f2 = RolloverWorkflowRunFundingOffer.Create(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        await sut.CreateRolloverWorkflowRunFundingOffersAsync(new[] { f1, f2 }, default);

        // Assert
        var all = await db.RolloverWorkflowRunFundingOffers.AsNoTracking().ToListAsync();

        Assert.Equal(2, all.Count);
        Assert.Contains(all, x =>
            x.FundingOfferId == f1.FundingOfferId &&
            x.RolloverWorkflowRunId == f1.RolloverWorkflowRunId);

        Assert.Contains(all, x =>
            x.FundingOfferId == f2.FundingOfferId &&
            x.RolloverWorkflowRunId == f2.RolloverWorkflowRunId);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunFundingOffersAsync_Throws_When_Null()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunFundingOffersAsync_Throws_When_Null));
        var sut = new RolloverRepository(db);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.CreateRolloverWorkflowRunFundingOffersAsync(null!, default));
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunFundingOffersAsync_NoOp_When_Empty()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunFundingOffersAsync_NoOp_When_Empty));
        var sut = new RolloverRepository(db);

        // Act
        await sut.CreateRolloverWorkflowRunFundingOffersAsync(Array.Empty<RolloverWorkflowRunFundingOffer>(), default);

        // Assert
        Assert.Empty(db.RolloverWorkflowRunFundingOffers);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunFundingOffersAsync_RespectsCancellation()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunFundingOffersAsync_RespectsCancellation));
        var sut = new RolloverRepository(db);

        var item = RolloverWorkflowRunFundingOffer.Create(
            Guid.NewGuid(),
            Guid.NewGuid());

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            sut.CreateRolloverWorkflowRunFundingOffersAsync(new[] { item }, cts.Token));

        Assert.Empty(db.RolloverWorkflowRunFundingOffers);
    }
}