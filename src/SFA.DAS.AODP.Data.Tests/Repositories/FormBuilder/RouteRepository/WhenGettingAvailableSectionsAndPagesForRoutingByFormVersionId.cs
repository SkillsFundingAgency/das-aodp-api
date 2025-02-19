using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.RouteRepository;

public class WhenGettingAvailableSectionsAndPagesForRoutingByFormVersionId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.RouteRepository _sut;

    public WhenGettingAvailableSectionsAndPagesForRoutingByFormVersionId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Available_Sections_And_Pages_For_Routing_By_Form_Version_Id()
    {
        // This is a placeholder

        // Arrange
        // Entities.Application.Application application = new();
        // var dbSet = new List<Entities.Application.Application>();

        // _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        // var result = await _sut.Create(application);

        // Assert
        // _context.Verify(c => c.Applications.AddAsync(application, default), Times.Once());
        // _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        // Assert.True(result.Id != Guid.Empty);
    }
}
