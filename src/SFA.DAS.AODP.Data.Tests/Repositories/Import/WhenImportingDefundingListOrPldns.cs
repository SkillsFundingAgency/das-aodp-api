using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Import;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Import;

public class WhenImportingDefundingListOrPldns
{
    private readonly Mock<IApplicationDbContext> mockContext = new();
    private readonly ImportRepository _sut;

    public WhenImportingDefundingListOrPldns() => _sut = new(mockContext.Object);

    [Fact]
    public async Task WhenTIsDefundingList_CallsAddRangeOnDefundingListsAndSaves()
    {
        // Arrange
        var items = new List<DefundingList>
            {
                new DefundingList { Qan = "Q1" },
                new DefundingList { Qan = "Q2" }
            };

        var mockDefundingSet = new Mock<DbSet<DefundingList>>();
        mockDefundingSet.Setup(d => d.AddRange(It.IsAny<IEnumerable<DefundingList>>())).Verifiable();

        mockContext.SetupGet(c => c.DefundingLists).Returns(mockDefundingSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1).Verifiable();

        // Act
        await _sut.BulkInsertAsync(items);

        // Assert
        mockDefundingSet.Verify(d => d.AddRange(It.Is<IEnumerable<DefundingList>>(x => x.SequenceEqual(items))), Times.Once);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenTIsPldns_CallsAddRangeOnPldnsAndSaves()
    {
        // Arrange
        var items = new List<Pldns>
            {
                new Pldns { Qan = "P1" },
                new Pldns { Qan = "P2" }
            };

        var mockPldnsSet = new Mock<DbSet<Pldns>>();
        mockPldnsSet.Setup(d => d.AddRange(It.IsAny<IEnumerable<Pldns>>())).Verifiable();

        mockContext.SetupGet(c => c.Pldns).Returns(mockPldnsSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1).Verifiable();

        // Act
        await _sut.BulkInsertAsync(items);

        // Assert
        mockPldnsSet.Verify(d => d.AddRange(It.Is<IEnumerable<Pldns>>(x => x.SequenceEqual(items))), Times.Once);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteDuplicateAsync_WhenContextIsNotApplicationDbContext_ThrowsInvalidOperationException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteDuplicateAsync("spName"));
    }
}
