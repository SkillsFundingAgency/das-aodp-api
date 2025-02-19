using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenMovingQuestionOrderUp
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenMovingQuestionOrderUp() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Moving_Question_Order_Up()
    {
        // Arrange
        Guid questionId1 = Guid.NewGuid();

        Guid questionId2 = Guid.NewGuid(); 

        Question newQuestion1 = new()
        {
            Id = questionId1,
            Order = 0      
        };

        Question newQuestion2 = new()
        {
            Id = questionId2,
            Order = 1
        };

        var dbSet = new List<Page>();

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.MoveQuestionOrderUp(questionId1);

        // Assert
        Assert.True(result);
        Assert.Equal(1, newQuestion1.Order);
        Assert.Equal(0, newQuestion2.Order);
    }
}