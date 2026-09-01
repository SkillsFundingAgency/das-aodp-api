using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverFundingEligibilityRepositoryTests
{
    [Fact]
    public async Task GetAsync_OfqualRequiresTheLatestEligibleVersion()
    {
        await using var context = CreateContext();
        var qualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var olderVersion = CreateVersion(qualificationId, 1);
        var latestVersion = CreateVersion(qualificationId, 2);
        context.QualificationVersions.AddRange(olderVersion, latestVersion);
        context.QualificationFundings.AddRange(
            CreateFunding(olderVersion.Id, fundingOfferId),
            CreateFunding(latestVersion.Id, fundingOfferId));
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sut = CreateSut(context);

        var results = await sut.GetAsync(
        [
            new FundingChangeKey(
                RolloverSourceTypes.Ofqual,
                olderVersion.Id,
                fundingOfferId,
                "2026/27"),
            new FundingChangeKey(
                RolloverSourceTypes.Ofqual,
                latestVersion.Id,
                fundingOfferId,
                "2026/27")
        ],
        TestContext.Current.CancellationToken);

        Assert.False(results.Single(x =>
            x.Key.SourceQualificationId == olderVersion.Id).IsEligible);
        Assert.True(results.Single(x =>
            x.Key.SourceQualificationId == latestVersion.Id).IsEligible);
    }

    [Fact]
    public async Task GetAsync_QaaUsesCandidateAcademicYearToAssessPersistentFunding()
    {
        await using var context = CreateContext();
        var qualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        context.QaaQualificationFundings.Add(QaaQualificationFunding.Create(
            qualificationId,
            fundingOfferId,
            new DateOnly(2026, 8, 1),
            new DateOnly(2027, 7, 31),
            "Not funded",
            new DateTime(2026, 8, 1)));
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sut = CreateSut(context);

        var results = await sut.GetAsync(
        [
            new FundingChangeKey(
                RolloverSourceTypes.Qaa,
                qualificationId,
                fundingOfferId,
                "2026/27"),
            new FundingChangeKey(
                RolloverSourceTypes.Qaa,
                qualificationId,
                fundingOfferId,
                "2027/28")
        ],
        TestContext.Current.CancellationToken);

        Assert.True(results.Single(x => x.AcademicYear == "2026/27").IsEligible);
        Assert.False(results.Single(x => x.AcademicYear == "2027/28").IsEligible);
    }

    [Fact]
    public async Task GetAsync_ThrowsForUnknownSourceType()
    {
        await using var context = CreateContext();
        var sut = CreateSut(context);

        await Assert.ThrowsAsync<NotSupportedException>(() => sut.GetAsync(
        [
            new FundingChangeKey(
                "FutureRegulator",
                Guid.NewGuid(),
                Guid.NewGuid(),
                "2026/27")
        ],
        TestContext.Current.CancellationToken));
    }

    private static QualificationVersions CreateVersion(Guid qualificationId, int version)
    {
        return new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualificationId,
            Version = version,
            EligibleForFunding = true,
            Status = "Available",
            Type = "Test",
            Ssa = "Test",
            Level = "3",
            SubLevel = "3",
            EqfLevel = "4",
            LastUpdatedDate = new DateTime(2026, 1, version),
            InsertedDate = new DateTime(2026, 1, version)
        };
    }

    private static QualificationFundings CreateFunding(
        Guid qualificationVersionId,
        Guid fundingOfferId)
    {
        return new QualificationFundings
        {
            Id = Guid.NewGuid(),
            QualificationVersionId = qualificationVersionId,
            FundingOfferId = fundingOfferId,
            EndDate = new DateOnly(2027, 7, 31)
        };
    }

    private static RolloverFundingEligibilityRepository CreateSut(
        ApplicationDbContext context)
    {
        var academicYearProvider = new Mock<IAcademicYearProvider>();
        academicYearProvider
            .Setup(x => x.GetCurrentAcademicYear())
            .Returns("2026/27");
        return new RolloverFundingEligibilityRepository(
            context,
            academicYearProvider.Object);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"RolloverFundingEligibility_{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }
}
