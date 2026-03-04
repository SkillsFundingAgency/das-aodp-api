using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class QaaRepositoryTests
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

        await _dbContext.RegulatedQaaQualifications.AddAsync(qaaQualification);
        await _dbContext.SaveChangesAsync();

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
}