using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.SectionRepository;

public class WhenCopyingSectionsForNewFormVersion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;

    private readonly Mock<IPageRepository> _pageRepository = new();

    public WhenCopyingSectionsForNewFormVersion() => _sut = new(
        _context.Object,
        _pageRepository.Object
    );

    [Fact]
    public async Task Then_Copy_Sections_For_New_Form()
    {
        // Arrange
        // Define Variables
        // Entities.Application.Application application = new();
        // var dbSet = new List<Entities.Application.Application>();

        // _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        // Run Function
        // var result = await _sut.Create(application);

        // Assert
        // Compare
        // _context.Verify(c => c.Applications.AddAsync(application, default), Times.Once());
        // _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        // Assert.True(result.Id != Guid.Empty);
    }
}
