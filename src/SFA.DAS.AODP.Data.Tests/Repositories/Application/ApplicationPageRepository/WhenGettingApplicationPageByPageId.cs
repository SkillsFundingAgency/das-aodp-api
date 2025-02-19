using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository
{
    public class WhenGettingApplicationPageByPageId
    {
        private readonly Mock<IApplicationDbContext> _context = new();

        private readonly Data.Repositories.Application.ApplicationPageRepository _sut;

        public WhenGettingApplicationPageByPageId() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_Get_ApplicationPage_By_PageId()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();

            ApplicationPage page = new()
            {
                ApplicationId = applicationId,
                PageId = pageId
            };

            var dbSet = new List<ApplicationPage>() { page };

            _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

            // Act
            var result = await _sut.GetApplicationPageByPageIdAsync(applicationId, pageId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(page, result);
        }
    }
}


