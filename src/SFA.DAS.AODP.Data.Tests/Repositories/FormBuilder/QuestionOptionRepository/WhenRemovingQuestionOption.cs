using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionOptionRepository;

public class WhenRemovingQuestionOption
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionOptionRepository _sut;

    public WhenRemovingQuestionOption() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Remove_QuestionOption()
    {
        // // Arrange
        // Guid questionOptionId = Guid.NewGuid();

        // QuestionOption newQuestionOption = new()
        // {
        //     Id = questionOptionId
        // };
        // var dbSet = new List<QuestionOption>{ newQuestionOption};

        // _context.SetupGet(c => c.QuestionOptions).ReturnsDbSet(dbSet);

        // // Act
        // var result = _sut.RemoveAsync(dbSet);

        // // Assert
        // _context.Verify(c => c.QuestionOptions.AddAsync(newQuestionOption, default), Times.Once());
        // _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        // Assert.Equal();
    }
}
