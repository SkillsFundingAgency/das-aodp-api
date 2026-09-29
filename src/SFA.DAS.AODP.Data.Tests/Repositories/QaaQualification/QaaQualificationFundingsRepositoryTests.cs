using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.QaaQualification;

public class QaaQualificationFundingsRepositoryTests : UnitTest
{
    private static readonly DateTime CreatedAt = new(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task CreateAsync_PersistsFundingToContext()
    {
        // Arrange
        await using var context = CreateContext();
        var sut = new QaaQualificationFundingsRepository(context);
        var funding = QaaQualificationFunding.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 8, 1),
            new DateOnly(2027, 7, 31),
            "Funded",
            CreatedAt);

        // Act
        await sut.CreateAsync(funding, CancellationToken);

        // Assert
        (await context.QaaQualificationFundings.CountAsync(CancellationToken)).ShouldBe(1);
        (await context.QaaQualificationFundings.SingleAsync(CancellationToken)).Id.ShouldBe(funding.Id);
    }

    [Fact]
    public async Task GetByQaaQualificationIdAsync_ReturnsOnlyFundingsForThatQualification()
    {
        // Arrange - GetByQaaQualificationIdAsync eager-loads the QaaQualification and FundingOffer
        // navigations, so both related rows need to exist for the funding rows to materialise.
        await using var context = CreateContext();
        var qualification = RegulatedQaaQualification.Create(
            CreatedAt, "AC1234", "Access to HE Diploma", "Awarding Body", new DateOnly(2026, 8, 1), new DateOnly(2027, 7, 31), SectorSubjectArea.Science);
        var otherQualification = RegulatedQaaQualification.Create(
            CreatedAt, "AC5678", "Access to HE Diploma", "Awarding Body", new DateOnly(2026, 8, 1), new DateOnly(2027, 7, 31), SectorSubjectArea.Science);
        var offer = new FundingOffer { Id = Guid.NewGuid(), Name = "Age 16-19", DisplayName = "Age 16-19" };
        context.RegulatedQaaQualifications.AddRange(qualification, otherQualification);
        context.FundingOffers.Add(offer);

        var matching = QaaQualificationFunding.Create(
            qualification.Id, offer.Id, null, null, "Funded", CreatedAt);
        var nonMatching = QaaQualificationFunding.Create(
            otherQualification.Id, offer.Id, null, null, "Funded", CreatedAt);
        context.QaaQualificationFundings.AddRange(matching, nonMatching);
        await context.SaveChangesAsync(CancellationToken);
        var sut = new QaaQualificationFundingsRepository(context);

        // Act
        var results = await sut.GetByQaaQualificationIdAsync(qualification.Id, CancellationToken);

        // Assert
        var result = results.ShouldHaveSingleItem();
        result.Id.ShouldBe(matching.Id);
    }

    [Fact]
    public async Task GetRolloverQaaQualificationFundingsAsync_DoesNotCrossMatchQualificationAndFundingOfferPairs()
    {
        // Arrange - two candidates with distinct (qualification, funding offer) pairs. A funding
        // record that mixes one candidate's qualification with the other's funding offer must not
        // be returned, even though both ids individually appear in the candidate list - the
        // repository re-filters by the exact pair, not the cross-product of the id lists.
        await using var context = CreateContext();
        var qualificationA = Guid.NewGuid();
        var offerX = Guid.NewGuid();
        var qualificationB = Guid.NewGuid();
        var offerY = Guid.NewGuid();

        var fundingForA = QaaQualificationFunding.Create(qualificationA, offerX, null, null, "Funded", CreatedAt);
        var fundingForB = QaaQualificationFunding.Create(qualificationB, offerY, null, null, "Funded", CreatedAt);
        var crossFunding = QaaQualificationFunding.Create(qualificationA, offerY, null, null, "Funded", CreatedAt);
        context.QaaQualificationFundings.AddRange(fundingForA, fundingForB, crossFunding);
        await context.SaveChangesAsync(CancellationToken);
        var sut = new QaaQualificationFundingsRepository(context);

        var candidates = new List<SourceQualificationFundingKey>
        {
            new(RolloverSourceTypes.Qaa, qualificationA, offerX, "2026/27"),
            new(RolloverSourceTypes.Qaa, qualificationB, offerY, "2026/27")
        };

        // Act
        var results = await sut.GetRolloverQaaQualificationFundingsAsync(candidates, CancellationToken);

        // Assert
        results.Select(x => x.Id).ShouldBe([fundingForA.Id, fundingForB.Id], ignoreOrder: true);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"QaaQualificationFundings_{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }
}
