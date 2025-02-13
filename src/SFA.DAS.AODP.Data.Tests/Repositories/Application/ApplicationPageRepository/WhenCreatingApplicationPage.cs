using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository
{
    public class WhenCreatingApplicationPage
    {
        private readonly Mock<IApplicationDbContext> _context = new();

        private readonly Data.Repositories.Application.ApplicationPageRepository _sut;

        public WhenCreatingApplicationPage() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_The_ApplicationPage_Is_Created()
        {
            // Arrange
            ApplicationPage page = new();
            var dbSet = new List<ApplicationPage>();

            _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

            // Act
            var result = await _sut.Create(page);

            // Assert
            _context.Verify(c => c.ApplicationPages.AddAsync(page, default), Times.Once());
            _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

            Assert.True(result.Id != Guid.Empty);
        }
    }
}


