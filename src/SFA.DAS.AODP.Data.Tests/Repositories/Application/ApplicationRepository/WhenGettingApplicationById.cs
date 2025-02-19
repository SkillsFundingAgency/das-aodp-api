using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;

public class WhenGettingApplicationById
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationRepository _sut;

    public WhenGettingApplicationById() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Application_By_Id()
    {
        // Arrange
        Guid applicationId = Guid.NewGuid();

        Entities.Application.Application application = new()
        {
            Id = applicationId,
        };

        var dbSet = new List<Entities.Application.Application>() { application };

        _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetByIdAsync(applicationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(application, result);
    }
}



