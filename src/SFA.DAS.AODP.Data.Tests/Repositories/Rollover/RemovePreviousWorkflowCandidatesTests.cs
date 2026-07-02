using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RemovePreviousWorkflowCandidatesTests
{
    private static ApplicationDbContext CreateDb(string name)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(name + "_" + Guid.NewGuid())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task RemovePreviousWorkflowCandidates_RemovesAll()
    {
        await using var db = CreateDb(nameof(RemovePreviousWorkflowCandidates_RemovesAll));

        var latestRun = Guid.NewGuid();
        var olderRun = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var c1 = RolloverWorkflowCandidate.Create(latestRun, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        var c2 = RolloverWorkflowCandidate.Create(olderRun, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);

        await db.RolloverWorkflowCandidates.AddRangeAsync(c1, c2);
        
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        await sut.DeleteAllWorkflowCandidatesAsync(CancellationToken.None);

        var remaining = await db.RolloverWorkflowCandidates.ToListAsync(TestContext.Current.CancellationToken);
        remaining.Count.ShouldBe(0);
    }

    [Fact]
    public async Task RemovePreviousWorkflowCandidates_NoLatestRun_DoesNothing()
    {
        await using var db = CreateDb(nameof(RemovePreviousWorkflowCandidates_NoLatestRun_DoesNothing));

        var run1 = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var c1 = RolloverWorkflowCandidate.Create(run1, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        await db.RolloverWorkflowCandidates.AddAsync(c1, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // clear runs table to simulate no runs
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await sut.DeleteAllWorkflowCandidatesAsync(TestContext.Current.CancellationToken);

        var remaining = await db.RolloverWorkflowCandidates.ToListAsync(TestContext.Current.CancellationToken);
        remaining.Count.ShouldBe(1);
    }
}