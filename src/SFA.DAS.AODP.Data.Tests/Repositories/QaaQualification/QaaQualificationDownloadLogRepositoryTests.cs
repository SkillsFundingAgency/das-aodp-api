using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.QaaQualification;

public class QaaQualificationDownloadLogRepositoryTests : UnitTest
{
    private readonly ApplicationDbContext _dbContext = new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);

    [Fact]
    public async Task CreateAsync_AddsEntry_AndReturnsGeneratedId()
    {
        // Arrange
        var sut = new QaaQualificationDownloadLogRepository(_dbContext);
        var entity = new QaaQualificationDownloadLog
        {
            UserDisplayName = "tester",
            DownloadDate = new DateTime(2026, 3, 1),
            FileName = "export.csv"
        };

        // Act
        var id = await sut.CreateAsync(entity, CancellationToken);

        // Assert
        id.ShouldNotBe(Guid.Empty);
        var saved = await _dbContext.QaaQualificationDownloadLog.SingleAsync(CancellationToken);
        saved.Id.ShouldBe(id);
        saved.UserDisplayName.ShouldBe("tester");
        saved.FileName.ShouldBe("export.csv");
    }

    [Fact]
    public async Task ListAsync_NoTakeSpecified_ReturnsAllOrderedByDownloadDateDescending()
    {
        // Arrange
        var sut = new QaaQualificationDownloadLogRepository(_dbContext);

        var older = new QaaQualificationDownloadLog { Id = Guid.NewGuid(), UserDisplayName = "user1", DownloadDate = new DateTime(2026, 1, 1) };
        var newer = new QaaQualificationDownloadLog { Id = Guid.NewGuid(), UserDisplayName = "user2", DownloadDate = new DateTime(2026, 2, 1) };

        await _dbContext.QaaQualificationDownloadLog.AddRangeAsync(older, newer);
        await _dbContext.SaveChangesAsync(CancellationToken);

        // Act
        var result = await sut.ListAsync(ct: CancellationToken);

        // Assert
        result.Count.ShouldBe(2);
        result[0].UserDisplayName.ShouldBe("user2");
        result[1].UserDisplayName.ShouldBe("user1");
    }

    [Fact]
    public async Task ListAsync_WithTake_LimitsResults()
    {
        // Arrange
        var sut = new QaaQualificationDownloadLogRepository(_dbContext);

        for (var i = 0; i < 15; i++)
        {
            await _dbContext.QaaQualificationDownloadLog.AddAsync(new QaaQualificationDownloadLog
            {
                Id = Guid.NewGuid(),
                UserDisplayName = $"user{i}",
                DownloadDate = new DateTime(2026, 1, 1).AddDays(i)
            });
        }
        await _dbContext.SaveChangesAsync(CancellationToken);

        // Act
        var result = await sut.ListAsync(take: 10, ct: CancellationToken);

        // Assert
        result.Count.ShouldBe(10);
        result[0].UserDisplayName.ShouldBe("user14");
    }
}
