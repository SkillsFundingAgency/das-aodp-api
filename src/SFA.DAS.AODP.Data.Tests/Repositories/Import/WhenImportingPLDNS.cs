using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Import;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Import;

public class WhenImportingPLDNS
{
    private readonly Mock<IApplicationDbContext> _context = new();
    private readonly PLDNSRepository _sut;

    public WhenImportingPLDNS() => _sut = new(_context.Object);

    [Fact]
    public async Task NullItems_DoesNotCallSaveChanges()
    {
        // Act
        await _sut.BulkInsertAsync(null!);

        // Assert
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _context.VerifyGet(c => c.PLDNS, Times.Never);
    }

    [Fact]
    public async Task WithItems_CallsAddRangeAndSaveChangesWithToken()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<PLDNS>>();

        _context.SetupGet(c => c.PLDNS).Returns(mockDbSet.Object);
        _context
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        var items = new List<PLDNS>
            {
                new PLDNS { Qan = "QAN-1" },
                new PLDNS { Qan = "QAN-2" }
            };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _sut.BulkInsertAsync(items, token);

        // Assert
        _context.VerifyGet(c => c.PLDNS, Times.Once);
        mockDbSet.Verify(d => d.AddRange(It.Is<IEnumerable<PLDNS>>(col => ReferenceEquals(col, items))), Times.Once);
        _context.Verify(c => c.SaveChangesAsync(It.Is<CancellationToken>(t => t == token)), Times.Once);
    }
}
