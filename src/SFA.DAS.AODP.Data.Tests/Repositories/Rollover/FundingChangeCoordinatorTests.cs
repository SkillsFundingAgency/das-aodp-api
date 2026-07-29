using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class FundingChangeCoordinatorTests
{
    [Fact]
    public async Task ExecuteAsync_RollsBackFundingMutationWhenReconciliationFails()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);

        var reconciler = new Mock<IRolloverCandidateReconciler>();
        reconciler
            .Setup(x => x.ReconcileAsync(
                It.IsAny<IReadOnlyCollection<FundingChangeKey>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Reconciliation failed."));
        var sut = new FundingChangeCoordinator(
            context,
            reconciler.Object,
            NullLogger<FundingChangeCoordinator>.Instance);
        var fundingOffer = new FundingOffer
        {
            Id = Guid.NewGuid(),
            Name = "Test funding offer",
            DisplayName = "Test funding offer"
        };
        var changeSet = FundingChangeSet.Create(
        [
            new FundingChangeKey("Ofqual", Guid.NewGuid(), fundingOffer.Id, "2026/27")
        ]);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.ExecuteAsync(
                changeSet,
                async ct =>
                {
                    context.FundingOffers.Add(fundingOffer);
                    await context.SaveChangesAsync(ct);
                    return true;
                },
                TestContext.Current.CancellationToken));

        context.ChangeTracker.Clear();
        Assert.Empty(await context.FundingOffers.ToListAsync(TestContext.Current.CancellationToken));
    }
}
