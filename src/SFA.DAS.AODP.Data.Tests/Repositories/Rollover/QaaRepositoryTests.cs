using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;
using SFA.DAS.AODP.Testing.Testing;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class QaaRepositoryTests : UnitTest
{
    private readonly ApplicationDbContext _dbContext = new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);

    [Fact]
    public async Task GetAllAsync_QualificationExists_ReturnsAll()
    {
        // Arrange
        var sut = new QaaQualificationRepository(_dbContext);

        var qaaQualification = RegulatedQaaQualification.Create(
            DateTime.UtcNow, "12345", "Test Qualification", "Test Awarding Body",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
            SectorSubjectArea.FromTiers("1", "1"));

        await _dbContext.RegulatedQaaQualifications.AddAsync(qaaQualification, CurrentContext.CancellationToken);
        await _dbContext.SaveChangesAsync(CurrentContext.CancellationToken);

        // Act
        var result = await sut.GetAllAsync(CancellationToken.None);

        // Assert
        var regulatedQaaQualifications = result.ToList();
        Assert.Single(regulatedQaaQualifications);
        var firstQualification = regulatedQaaQualifications.First();

        Assert.Equal(qaaQualification.DateOfDataSnapshot, firstQualification.DateOfDataSnapshot);
        Assert.Equal(qaaQualification.AimCode, firstQualification.AimCode);
        Assert.Equal(qaaQualification.QualificationTitle, firstQualification.QualificationTitle);
        Assert.Equal(qaaQualification.AwardingBody, firstQualification.AwardingBody);
        Assert.Equal(qaaQualification.StartDate, firstQualification.StartDate);
        Assert.Equal(qaaQualification.LastDateForRegistration, firstQualification.LastDateForRegistration);
        Assert.Equal(qaaQualification.Level, firstQualification.Level);
        Assert.Equal(qaaQualification.Type, firstQualification.Type);
        Assert.Equal(qaaQualification.Status, firstQualification.Status);
    }

    [Fact]
    public async Task GetAllAsync_NoQualificationExists_ReturnsEmptyCollection()
    {
        // Arrange
        var sut = new QaaQualificationRepository(_dbContext);

        // Act
        var result = await sut.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SaveChanges_ShouldReturnCompletedTask()
    {
        // Arrange
        var sut = new QaaQualificationRepository(_dbContext);

        // Act
        var result = sut.SaveChangesAsync(CancellationToken.None);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public async Task GetSummaryCountsAsync_CountsNewExtendedAndDiscontinued_AndReturnsLatestSnapshotDate()
    {
        // Arrange
        var sut = new QaaQualificationRepository(_dbContext);

        var earlierSnapshot = new DateTime(2026, 1, 1);
        var latestSnapshot = new DateTime(2026, 2, 17);

        var newQual = RegulatedQaaQualification.Create(
            latestSnapshot, "AIM001", "New Qual", "Awarding Body A",
            DateOnly.FromDateTime(latestSnapshot), DateOnly.FromDateTime(latestSnapshot.AddYears(1)),
            SectorSubjectArea.FromTiers("1", "1"))
            .SetChangeOutcome(QaaImportComparisonOutcome.New, QaaLastDateForRegistrationChangeType.NotChanged);

        var extendedQual = RegulatedQaaQualification.Create(
            earlierSnapshot, "AIM002", "Extended Qual", "Awarding Body B",
            DateOnly.FromDateTime(earlierSnapshot), DateOnly.FromDateTime(earlierSnapshot.AddYears(1)),
            SectorSubjectArea.FromTiers("1", "1"))
            .SetChangeOutcome(QaaImportComparisonOutcome.LastDateForRegistrationChanged, QaaLastDateForRegistrationChangeType.Extended);

        var unchangedQual = RegulatedQaaQualification.Create(
            earlierSnapshot, "AIM003", "Unchanged Qual", "Awarding Body C",
            DateOnly.FromDateTime(earlierSnapshot), DateOnly.FromDateTime(earlierSnapshot.AddYears(1)),
            SectorSubjectArea.FromTiers("1", "1"))
            .SetChangeOutcome(QaaImportComparisonOutcome.NotChanged, QaaLastDateForRegistrationChangeType.NotChanged);

        var discontinuedQual = RegulatedQaaQualification.Create(
            earlierSnapshot, "AIM004", "Discontinued Qual", "Awarding Body D",
            DateOnly.FromDateTime(earlierSnapshot), DateOnly.FromDateTime(earlierSnapshot.AddYears(1)),
            SectorSubjectArea.FromTiers("1", "1"))
            .SetChangeOutcome(QaaImportComparisonOutcome.NotChanged, QaaLastDateForRegistrationChangeType.NotChanged);
        typeof(RegulatedQaaQualification).GetProperty(nameof(RegulatedQaaQualification.IsDiscontinued))!
            .SetValue(discontinuedQual, true);

        await _dbContext.RegulatedQaaQualifications.AddRangeAsync(newQual, extendedQual, unchangedQual, discontinuedQual);
        await _dbContext.SaveChangesAsync(CurrentContext.CancellationToken);

        // Act
        var result = await sut.GetSummaryCountsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(latestSnapshot, result.DataLastImportedDate);
        Assert.Equal(1, result.NewCount);
        Assert.Equal(1, result.ExtendedCount);
        Assert.Equal(1, result.DiscontinuedCount);
    }

    [Fact]
    public async Task GetSummaryCountsAsync_NoQualifications_ReturnsZeroCountsAndNullDate()
    {
        // Arrange
        var sut = new QaaQualificationRepository(_dbContext);

        // Act
        var result = await sut.GetSummaryCountsAsync(CancellationToken.None);

        // Assert
        Assert.Null(result.DataLastImportedDate);
        Assert.Equal(0, result.NewCount);
        Assert.Equal(0, result.ExtendedCount);
        Assert.Equal(0, result.DiscontinuedCount);
    }
}