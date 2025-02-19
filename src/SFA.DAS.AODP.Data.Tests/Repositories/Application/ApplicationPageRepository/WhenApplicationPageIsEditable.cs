using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository
{
    public class WhenApplicationPageIsEditable
    {
        private readonly Mock<IApplicationDbContext> _context = new();
        private readonly Data.Repositories.Application.ApplicationPageRepository _sut;
        public WhenApplicationPageIsEditable() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_Check_If_ApplicationPage_Is_Editable()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();
            Guid pageId = Guid.NewGuid();

            ApplicationPage page = new()
            {
                ApplicationId = applicationId,
                Id = pageId,
                Application = new() { Submitted = false }
            };

            var dbSet = new List<ApplicationPage>() { page };

            _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

            // Act
            // var result = await _sut.IsApplicationPageEditable(v => v.Id == id && v.Application.Submitted == false);

            var result = await _sut.IsApplicationPageEditable(pageId);

            // Assert
            Assert.True(result);
        } 
    }
}