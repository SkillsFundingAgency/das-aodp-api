using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository
{
    public class WhenUpsertingPage
    {
        private readonly Mock<IApplicationDbContext> _context = new();

        private readonly Data.Repositories.Application.ApplicationPageRepository _sut;

        public WhenUpsertingPage() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_Page_Is_Inserted()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();

            ApplicationPage page = new();
            
            var dbSet = new List<ApplicationPage>() { page };

            _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

            // Act
            await _sut.UpsertAsync(page);

            // Assert
            _context.Verify(c => c.ApplicationPages.Add(page), Times.Once());
            _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task Then_Page_Is_Updated()
        {
            // Arrange
            ApplicationPage page = new() { Id = Guid.NewGuid() };

            var dbSet = new List<ApplicationPage>();

            _context.Setup(c => c.ApplicationPages).ReturnsDbSet(dbSet);

            // Act
            await _sut.UpsertAsync(page);

            // Assert
            _context.Verify(c => c.ApplicationPages.Update(page), Times.Once());
            _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
    }
}


