using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository
{
    // This test shouldn't currently pass due to GetByOrganisationId missing filter
    public class WhenGettingSectionSummaryForApplication
    {
        // View_SectionSummaryForApplication
        private readonly Mock<IApplicationDbContext> _context = new();

        private readonly Data.Repositories.Application.ApplicationRepository _sut;

        public WhenGettingSectionSummaryForApplication() => _sut = new(_context.Object);

        [Fact]
        public async Task Then_Get_Section_Summary_For_Application()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();

            Entities.Application.Application application = new()
            {
                Id = applicationId,
            };

            var sectionSummaries = new List<View_SectionSummaryForApplication>() {
                new View_SectionSummaryForApplication{
                    ApplicationId = applicationId
                }
            };

            var dbSet = new List<Entities.Application.Application>() { application };

            _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

            // Act
            var result = await _sut.GetSectionSummaryByApplicationIdAsync(applicationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sectionSummaries, result);
        }

        // public async Task<List<View_SectionSummaryForApplication>> GetSectionSummaryByApplicationIdAsync(Guid applicationId)
        // {
        //     return await _context
        //     .View_SectionSummaryForApplications
        //     .Where(a => a.ApplicationId == applicationId)
        //     .ToListAsync();
        // }
    }
}


