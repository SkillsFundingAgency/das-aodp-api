using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationMessagesRepository
{
    public class WhenGettingApplicationMessageById
    {
        private readonly Mock<IApplicationDbContext> _context = new();

        private readonly Data.Repositories.Application.ApplicationMessagesRepository _sut;

        public WhenGettingApplicationMessageById() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_Get_ApplicationMessage_By_Id()
        {
            // Arrange
            Guid messageId = Guid.NewGuid();

            Message message = new()
            {
                Id = messageId,
            };

            var dbSet = new List<Message>() { message };

            _context.SetupGet(c => c.Messages).ReturnsDbSet(dbSet);

            // Act
            var result = await _sut.GetByIdAsync(messageId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(message, result);
        }
    }
}


