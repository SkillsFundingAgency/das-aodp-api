using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Pldns;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class PldnsRepositoryTests : UnitTest
{
    private readonly ApplicationDbContext _dbContext = new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);

    [Fact]
    public async Task GetAllAsync_NoRecordsReturned()
    {
        // Arrange
        var sut = new PldnsRepository(_dbContext);

        // Act
        var result = await sut.GetAllAsync(CancellationToken);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_RecordExists_RecordsReturned()
    {
        // Arrange
        var sut = new PldnsRepository(_dbContext);

        var pldns = new Pldns
        {
            Id = 1,
            Qan = "123456",
            ImportDate = new DateTime(2026, 01, 01),
            Pldns16To19 = new DateTime(2025, 10, 08)
        };

        await _dbContext.Pldns.AddAsync(pldns, CancellationToken);
        await _dbContext.SaveChangesAsync(CancellationToken);

        // Act
        var result = await sut.GetAllAsync(CancellationToken);

        // Assert
        result.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetPldnsByQanAsync_RecordExists_RecordsReturned()
    {
        // Arrange
        var sut = new PldnsRepository(_dbContext);

        var pldns = new Pldns
        {
            Id = 1,
            Qan = "123456",
            ImportDate = new DateTime(2026, 01, 01),
            Pldns16To19 = new DateTime(2025, 10, 08)
        };

        await _dbContext.Pldns.AddAsync(pldns, CancellationToken);
        await _dbContext.SaveChangesAsync(CancellationToken);

        // Act
        var result = await sut.GetPldnsByQanAsync("123456", CancellationToken);

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetPldnsByQanAsync_NoRecordExists_NoRecordsReturned()
    {
        // Arrange
        var sut = new PldnsRepository(_dbContext);

        // Negative data
        var pldns = new Pldns
        {
            Id = 1,
            Qan = "123456",
            ImportDate = new DateTime(2026, 01, 01),
            Pldns16To19 = new DateTime(2025, 10, 08)
        };

        await _dbContext.Pldns.AddAsync(pldns, CancellationToken);
        await _dbContext.SaveChangesAsync(CancellationToken);

        // Act
        var result = await sut.GetPldnsByQanAsync("444444", CancellationToken);

        // Assert
        result.ShouldBeNull();
    }
}