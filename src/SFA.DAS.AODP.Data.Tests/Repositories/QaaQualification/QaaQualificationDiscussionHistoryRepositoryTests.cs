using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.QaaQualification;

public class QaaQualificationDiscussionHistoryRepositoryTests : UnitTest
{
    [Fact]
    public async Task CreateAsync_AssignsIdAndPersistsHistory()
    {
        // Arrange
        await using var context = CreateContext();
        var sut = new QaaQualificationDiscussionHistoryRepository(context);
        var history = new QaaQualificationDiscussionHistory
        {
            QaaQualificationId = Guid.NewGuid(),
            ActionTypeId = Guid.NewGuid(),
            Notes = "Reviewed"
        };

        // Act
        await sut.CreateAsync(history);

        // Assert
        history.Id.ShouldNotBe(Guid.Empty);
        (await context.QaaQualificationDiscussionHistory.CountAsync(CancellationToken)).ShouldBe(1);
        (await context.QaaQualificationDiscussionHistory.SingleAsync(CancellationToken)).Id.ShouldBe(history.Id);
    }

    [Fact]
    public async Task AddDiscussionHistories_StagesHistoriesWithoutSaving()
    {
        // Arrange - this method only stages the entities on the change tracker; the caller is
        // responsible for calling SaveChangesAsync as part of a larger unit of work.
        await using var context = CreateContext();
        var sut = new QaaQualificationDiscussionHistoryRepository(context);
        var histories = new List<QaaQualificationDiscussionHistory>
        {
            new() { Id = Guid.NewGuid(), QaaQualificationId = Guid.NewGuid(), ActionTypeId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), QaaQualificationId = Guid.NewGuid(), ActionTypeId = Guid.NewGuid() }
        };

        // Act
        sut.AddDiscussionHistories(histories);

        // Assert - nothing persisted yet
        (await context.QaaQualificationDiscussionHistory.CountAsync(CancellationToken)).ShouldBe(0);

        // Act - the caller completes the unit of work
        await context.SaveChangesAsync(CancellationToken);

        // Assert - now persisted
        (await context.QaaQualificationDiscussionHistory.CountAsync(CancellationToken)).ShouldBe(2);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"QaaQualificationDiscussionHistory_{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }
}
