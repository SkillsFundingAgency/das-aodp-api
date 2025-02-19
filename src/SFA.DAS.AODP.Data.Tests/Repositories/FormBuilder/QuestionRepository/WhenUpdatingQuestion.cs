using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenUpdatingQuestion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenUpdatingQuestion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Update_Question()
    {
        // Arrange
        Question newQuestion = new();
        var dbSet = new List<Question>();

        _context.SetupGet(c => c.Questions).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.Update(newQuestion);

        // Assert
        _context.Verify(c => c.Questions.Update(newQuestion), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
