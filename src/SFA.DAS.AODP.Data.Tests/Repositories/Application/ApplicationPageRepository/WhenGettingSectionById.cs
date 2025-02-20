using Azure;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository
{
    public class WhenGettingSectionById
    {
        private readonly Mock<IApplicationDbContext> _context = new();

        private readonly Data.Repositories.Application.ApplicationPageRepository _sut;

        public WhenGettingSectionById() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_Get_Section_By_Id()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();
            Guid sectionId = Guid.NewGuid();

            Page newPage = new()
            {
                Id = pageId,
                SectionId = sectionId
            };

            ApplicationPage page = new()
            {
                ApplicationId = applicationId,
                PageId = pageId,
                Page = newPage
            };

            var dbSet = new List<ApplicationPage>() { page };

            _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

            // Act
            var result = await _sut.GetBySectionIdAsync(sectionId, applicationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dbSet, result);
        }
    }
}


