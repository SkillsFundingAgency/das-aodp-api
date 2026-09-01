using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories;

public class AllQualificationFundingsRepositoryTests
{
    [Fact]
    public async Task QaaQualificationFunding_CanBeStoredAgainstQaaQualification()
    {
        await using var context = CreateInMemoryContext();

        var qaaQualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var qaaQualification = CreateQaaQualification(qaaQualificationId);
        var fundingOffer = new FundingOffer
        {
            Id = fundingOfferId,
            Name = "Age1619",
            DisplayName = "Age 16-19"
        };

        context.RegulatedQaaQualifications.Add(qaaQualification);
        context.FundingOffers.Add(fundingOffer);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var funding = QaaQualificationFunding.Create(
            qaaQualificationId,
            fundingOfferId,
            new DateOnly(2025, 8, 1),
            new DateOnly(2026, 7, 31),
            "Approved",
            DateTime.UtcNow);

        var repository = new QaaQualificationFundingsRepository(context);

        await repository.CreateAsync(funding, TestContext.Current.CancellationToken);
        var result = await repository.GetByQaaQualificationIdAsync(qaaQualificationId, TestContext.Current.CancellationToken);

        Assert.Single(result);
        Assert.Equal(qaaQualificationId, result[0].QaaQualificationId);
        Assert.Equal(fundingOfferId, result[0].FundingOfferId);
        Assert.Equal("Access to Higher Education Diploma (Science)", result[0].QaaQualification.QualificationTitle);
    }

    [Fact]
    public async Task GetAsync_ReturnsOfqualAndQaaRows_WithFilters()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        await CreateAllQualificationFundingsView(context);

        var repository = new AllQualificationFundingsRepository(context);

        var allRows = await repository.GetAsync(new AllQualificationFundingFilter(), TestContext.Current.CancellationToken);
        var qaaRows = await repository.GetAsync(new AllQualificationFundingFilter
        {
            SourceType = "QAA",
            FundingOfferId = Guid.Parse("00000000-0000-0000-0000-000000000007")
        }, TestContext.Current.CancellationToken);

        Assert.Equal(2, allRows.Count);
        Assert.Contains(allRows, row => row.SourceType == "Ofqual");
        Assert.Contains(allRows, row => row.SourceType == "QAA");

        var qaaRow = Assert.Single(qaaRows);
        Assert.Equal("QAA", qaaRow.SourceType);
        Assert.Equal("Z1234567", qaaRow.QualificationReference);
        Assert.Equal("QAA qualification", qaaRow.QualificationName);
    }

    [Fact]
    public void AllQualificationFundings_IsKeylessReadModel()
    {
        using var context = CreateInMemoryContext();

        var entityType = context.Model.FindEntityType(typeof(AllQualificationFunding));

        Assert.NotNull(entityType);
        Assert.Null(entityType!.FindPrimaryKey());
        Assert.Equal("AllQualificationFundings", entityType.GetViewName());
        Assert.Equal("funded", entityType.GetViewSchema());
    }

    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static RegulatedQaaQualification CreateQaaQualification(Guid id)
    {
        var qualification = RegulatedQaaQualification.Create(
            new DateTime(2025, 1, 1),
            "Z1234567",
            "Access to Higher Education Diploma (Science)",
            "Test Awarding Body",
            new DateOnly(2025, 8, 1),
            new DateOnly(2026, 7, 31),
            SectorSubjectArea.FromTiers("1", "1"));

        typeof(RegulatedQaaQualification)
            .GetProperty(nameof(RegulatedQaaQualification.Id))!
            .SetValue(qualification, id);

        return qualification;
    }

    private static async Task CreateAllQualificationFundingsView(ApplicationDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync(
            """
            CREATE VIEW AllQualificationFundings AS
            SELECT
                '10000000-0000-0000-0000-000000000001' AS FundingId,
                'Ofqual' AS SourceType,
                '20000000-0000-0000-0000-000000000001' AS SourceQualificationId,
                'OFQ123' AS QualificationReference,
                '00000000-0000-0000-0000-000000000006' AS FundingOfferId,
                'AdvancedLearnerLoans' AS FundingStreamName,
                NULL AS FundingApprovalStartDate,
                NULL AS FundingApprovalEndDate,
                NULL AS FundingStatus,
                'Ofqual qualification' AS QualificationName,
                'Level 2' AS Level,
                'Ofqual awarding organisation' AS AwardingOrganisationName
            UNION ALL
            SELECT
                '10000000-0000-0000-0000-000000000002' AS FundingId,
                'QAA' AS SourceType,
                '20000000-0000-0000-0000-000000000002' AS SourceQualificationId,
                'Z1234567' AS QualificationReference,
                '00000000-0000-0000-0000-000000000007' AS FundingOfferId,
                'Age1619' AS FundingStreamName,
                NULL AS FundingApprovalStartDate,
                NULL AS FundingApprovalEndDate,
                'Approved' AS FundingStatus,
                'QAA qualification' AS QualificationName,
                'Level 3' AS Level,
                'QAA awarding body' AS AwardingOrganisationName
            """,
            TestContext.Current.CancellationToken);
    }
}
