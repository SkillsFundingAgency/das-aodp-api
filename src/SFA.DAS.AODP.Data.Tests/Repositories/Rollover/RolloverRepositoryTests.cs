using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using Moq;
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
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsEmptyResult_When_DbSetIsNull()
    {
        // Arrange
        var contextMock = _fixture.Freeze<Mock<IApplicationDbContext>>();
        contextMock.Setup(c => c.RolloverWorkflowCandidates).Returns((DbSet<Data.Entities.Rollover.RolloverWorkflowCandidate>?)null);

        var sut = new RolloverRepository(contextMock.Object);

        // Act
        var result = await sut.GetAllRolloverWorkflowCandidatesAsync(0, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalRecords);
        Assert.Equal(0, result.Skip);
        Assert.Equal(10, result.Take);
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
        var result = await sut.GetAllRolloverWorkflowCandidatesAsync(0, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalRecords);
        Assert.Equal(0, result.Skip);
        Assert.Equal(10, result.Take);
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
            var result = await sut.GetAllRolloverWorkflowCandidatesAsync(1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalRecords);
            Assert.Equal(1, result.Skip);
            Assert.Equal(1, result.Take);
            Assert.Single(result.Data);

            var returned = result.Data.Single();
            Assert.Equal(e2.Id, returned.Id);
            Assert.Equal(e2.QualificationVersionId, returned.QualificationVersionId);
            Assert.Equal(e2.FundingOfferId, returned.FundingOfferId);
            Assert.Equal(e2.AcademicYear, returned.AcademicYear);
        }
    }

    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_Throws_When_AsyncEnumerationFails()
    {
        // Arrange
        var contextMock = _fixture.Freeze<Mock<IApplicationDbContext>>();

        var now = DateTime.UtcNow;
        var entity = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", now, null, now);
        var list = new List<Data.Entities.Rollover.RolloverWorkflowCandidate> { entity };
        var queryable = list.AsQueryable();

        var dbSetMock = new Mock<DbSet<Data.Entities.Rollover.RolloverWorkflowCandidate>>();

        dbSetMock.As<IQueryable<Data.Entities.Rollover.RolloverWorkflowCandidate>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSetMock.As<IQueryable<Data.Entities.Rollover.RolloverWorkflowCandidate>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<Data.Entities.Rollover.RolloverWorkflowCandidate>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock.As<IQueryable<Data.Entities.Rollover.RolloverWorkflowCandidate>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        dbSetMock.As<IAsyncEnumerable<Data.Entities.Rollover.RolloverWorkflowCandidate>>()
                 .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                 .Throws(new Exception("Async enumeration failure"));

        contextMock.Setup(c => c.RolloverWorkflowCandidates).Returns(dbSetMock.Object);

        var sut = new RolloverRepository(contextMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetAllRolloverWorkflowCandidatesAsync(0, 10));
        Assert.Contains("IAsyncQueryProvider", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}