using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenCreatingQuestion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenCreatingQuestion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Create_Question()
    {
        // Arrange
        Question newQuestion = new();
        var dbSet = new List<Question>();

        _context.SetupGet(c => c.Questions).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.Create(newQuestion);

        // Assert
        _context.Verify(c => c.Questions.AddAsync(newQuestion, default), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);
    }
}
