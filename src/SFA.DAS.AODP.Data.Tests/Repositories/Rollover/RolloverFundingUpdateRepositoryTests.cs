using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverFundingUpdateRepositoryTests
{
    [Fact]
    public async Task GetFundingUpdatesAsync_ReturnsSourceSpecificUpdates_WhenSourcesShareIds()
    {
        await using var context = CreateInMemoryContext();
        var repository = new RolloverFundingUpdateRepository(context);

        var sourceQualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var existingEndDate = new DateOnly(2026, 7, 31);
        var updatedAt = new DateTime(2026, 8, 1, 10, 0, 0);

        var ofqualFunding = new QualificationFundings
        {
            Id = Guid.NewGuid(),
            QualificationVersionId = sourceQualificationId,
            FundingOfferId = fundingOfferId,
            EndDate = existingEndDate
        };

        var qaaFunding = QaaQualificationFunding.Create(
            sourceQualificationId,
            fundingOfferId,
            new DateOnly(2026, 8, 1),
            existingEndDate,
            "Approved",
            new DateTime(2026, 1, 1, 9, 0, 0));

        await context.QualificationFundings.AddAsync(ofqualFunding, TestContext.Current.CancellationToken);
        await context.QaaQualificationFundings.AddAsync(qaaFunding, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var keys = new List<SourceQualificationFundingKey>
        {
            new(RolloverSourceTypes.Ofqual, sourceQualificationId, fundingOfferId, "2026/27"),
            new(RolloverSourceTypes.Qaa, sourceQualificationId, fundingOfferId, "2026/27")
        };

        var result = await repository.GetFundingUpdatesAsync(keys, TestContext.Current.CancellationToken);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.SourceType == RolloverSourceTypes.Ofqual);
        Assert.Contains(result, x => x.SourceType == RolloverSourceTypes.Qaa);

        result.Single(x => x.SourceType == RolloverSourceTypes.Ofqual)
            .ApplyFundingEndDate(new DateOnly(2027, 7, 31), "Ofqual comment", updatedAt);

        result.Single(x => x.SourceType == RolloverSourceTypes.Qaa)
            .ApplyFundingEndDate(new DateOnly(2028, 7, 31), "QAA comment", updatedAt);

        Assert.Equal(new DateOnly(2027, 7, 31), ofqualFunding.EndDate);
        Assert.Equal("Ofqual comment", ofqualFunding.Comments);
        Assert.Equal(new DateOnly(2028, 7, 31), qaaFunding.EndDate);
        Assert.Equal("QAA comment", qaaFunding.Comments);
        Assert.Equal(updatedAt, qaaFunding.UpdatedAt);
    }

    [Fact]
    public async Task GetFundingUpdatesAsync_UsesCandidateYearWithPersistentQaaFunding()
    {
        await using var context = CreateInMemoryContext();
        var repository = new RolloverFundingUpdateRepository(context);
        var sourceQualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var originalEndDate = new DateOnly(2027, 7, 31);

        var selectedFunding = QaaQualificationFunding.Create(
            sourceQualificationId,
            fundingOfferId,
            new DateOnly(2026, 8, 1),
            originalEndDate,
            "Approved",
            DateTime.UtcNow);
        await context.QaaQualificationFundings.AddAsync(
            selectedFunding,
            TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await repository.GetFundingUpdatesAsync(
            [new(RolloverSourceTypes.Qaa, sourceQualificationId, fundingOfferId, "2026/27")],
            TestContext.Current.CancellationToken);

        var update = Assert.Single(result);
        Assert.Equal("2026/27", update.AcademicYear);
        update.ApplyFundingEndDate(new DateOnly(2027, 8, 31), "Updated", DateTime.UtcNow);

        Assert.Equal(new DateOnly(2027, 8, 31), selectedFunding.EndDate);
    }

    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
