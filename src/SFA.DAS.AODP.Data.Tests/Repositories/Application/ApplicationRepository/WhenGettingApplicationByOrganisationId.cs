using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;
public class WhenGettingApplicationByOrganisationId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationRepository _sut;

    public WhenGettingApplicationByOrganisationId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Application_By_OrganisationId()
    {
        // Arrange
        Guid applicationId = Guid.NewGuid();
        Guid organisationId = Guid.NewGuid();

        Entities.Application.Application application = new()
        {
            OrganisationId = organisationId,
            Id = applicationId
        };

        var dbSet = new List<Entities.Application.Application>() { application };

        _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetByOrganisationId(organisationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dbSet, result);
    }
}


