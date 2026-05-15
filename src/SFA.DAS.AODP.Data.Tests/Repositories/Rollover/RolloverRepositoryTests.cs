using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverRepositoryTests
{
    private readonly IFixture _fixture;

    public RolloverRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
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
    public async Task UpdateRolloverWorkflowCandidatesAsync_UpdatesEntities()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_Update_Entities_" + Guid.NewGuid())
            .Options;

        var now = DateTime.UtcNow;
        var e1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now.AddDays(-1), null, now.AddDays(-1));

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidates.AddAsync(e1);
            await db.SaveChangesAsync();
        }

        RolloverWorkflowCandidate detached;
        await using (var dbRead = new ApplicationDbContext(options))
        {
            detached = await dbRead.RolloverWorkflowCandidates.AsNoTracking().FirstAsync();
        }

        var beforeUpdate = detached.UpdatedAt;

        detached.SetP1Result(true, null);

        await using (var dbUpdate = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(dbUpdate);

            // Act
            await sut.UpdateRolloverWorkflowCandidatesAsync(new[] { detached }, CancellationToken.None);
        }

        // Assert
        await using (var dbAssert = new ApplicationDbContext(options))
        {
            var saved = await dbAssert.RolloverWorkflowCandidates.AsNoTracking().FirstAsync();
            Assert.True(saved.PassP1);
            Assert.Null(saved.P1FailureReason);
            Assert.True(saved.UpdatedAt > beforeUpdate);
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
        var result = (await sut.GetRolloverWorkflowCandidatesP1ChecksAsync(CancellationToken.None)).ToList();

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
            ThresholdDate = now.Date.AddDays(-10),
            LatestFundingApprovalEndDate = now.Date.AddDays(-20),
            OperationalStartDate = now.Date.AddYears(-1),
            OperationalEndDate = now.Date.AddMonths(6),
            OfferedInEngland = true,
            Glh = 150,
            Tqt = 200,
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
            ThresholdDate = now.Date.AddDays(30),
            LatestFundingApprovalEndDate = now.Date.AddDays(60),
            OperationalStartDate = now.Date,
            OperationalEndDate = null,
            OfferedInEngland = false,
            Glh = 50,
            Tqt = 50,
            IsOnDefundingList = true
        };

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidatesP1Checks.AddRangeAsync(new[] { e1, e2 });
            await db.SaveChangesAsync();
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act
            var result = (await sut.GetRolloverWorkflowCandidatesP1ChecksAsync(default)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.WorkflowCandidateId == e1.WorkflowCandidateId);
            Assert.Contains(result, r => r.WorkflowCandidateId == e2.WorkflowCandidateId);

            var fetched1 = result.Single(r => r.WorkflowCandidateId == e1.WorkflowCandidateId);
            Assert.Equal(e1.AcademicYear, fetched1.AcademicYear);
            Assert.Equal(e1.FundingStream, fetched1.FundingStream);
            Assert.Equal(e1.Glh, fetched1.Glh);
            Assert.Equal(e1.Tqt, fetched1.Tqt);
            Assert.Equal(e1.IsOnDefundingList, fetched1.IsOnDefundingList);

            var fetched2 = result.Single(r => r.WorkflowCandidateId == e2.WorkflowCandidateId);
            Assert.Equal(e2.AcademicYear, fetched2.AcademicYear);
            Assert.Equal(e2.FundingStream, fetched2.FundingStream);
            Assert.Equal(e2.Glh, fetched2.Glh);
            Assert.Equal(e2.Tqt, fetched2.Tqt);
            Assert.Equal(e2.IsOnDefundingList, fetched2.IsOnDefundingList);
        }
    }
}