using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenValidatingQuestionForChange
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenValidatingQuestionForChange() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Validate_Question_For_Change()
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
