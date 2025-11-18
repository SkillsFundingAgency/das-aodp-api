using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Import;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Import;

public class WhenImportingDefundingList
{
    private readonly Mock<IApplicationDbContext> _context = new();
    private readonly DefundingListRepository _sut;

    public WhenImportingDefundingList() => _sut = new(_context.Object);

    [Fact]
    public async Task NullItems_DoesNotCallSaveChanges()
    {
        // Arrange

        // Act
        await _sut.BulkInsertAsync(null!);

        // Assert
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _context.VerifyGet(c => c.DefundingLists, Times.Never);
    }

    [Fact]
    public async Task WithItems_CallsAddRangeAndSaveChangesWithToken()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<DefundingList>>();

        _context.SetupGet(c => c.DefundingLists).Returns(mockDbSet.Object);
        _context
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        var items = new List<DefundingList>
            {
                new DefundingList { Qan = "QAN-1" },
                new DefundingList { Qan = "QAN-2" }
            };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _sut.BulkInsertAsync(items, token);

        // Assert
        _context.VerifyGet(c => c.DefundingLists, Times.Once);
        mockDbSet.Verify(d => d.AddRange(It.Is<IEnumerable<DefundingList>>(col => ReferenceEquals(col, items))), Times.Once);
        _context.Verify(c => c.SaveChangesAsync(It.Is<CancellationToken>(t => t == token)), Times.Once);
    }
}
