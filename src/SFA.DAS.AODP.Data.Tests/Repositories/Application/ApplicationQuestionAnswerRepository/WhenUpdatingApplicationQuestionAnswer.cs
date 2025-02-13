using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationQuestionAnswerRepository
{
    public class WhenUpdatingApplicationQuestionAnswer
    {
        private readonly Mock<IApplicationDbContext> _context = new();

        private readonly Data.Repositories.Application.ApplicationQuestionAnswerRepository _sut;

        public WhenUpdatingApplicationQuestionAnswer() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_The_ApplicationQuestionAnswer_Is_Updated()
        {
            // Arrange
            ApplicationQuestionAnswer questionAnswer = new();
            var dbSet = new List<ApplicationQuestionAnswer>();

            _context.SetupGet(c => c.ApplicationQuestionAnswers).ReturnsDbSet(dbSet);

            // Act
            var result = await _sut.Update(questionAnswer);

            // Assert
            _context.Verify(c => c.ApplicationQuestionAnswers.Update(questionAnswer), Times.Once());
            _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
    }
}


