// using Moq;
// using Moq.EntityFrameworkCore;
// using SFA.DAS.AODP.Data.Context;
// using SFA.DAS.AODP.Data.Entities.Application;

// namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository
// {
//     public class WhenUpsertingPage
//     {
//         private readonly Mock<IApplicationDbContext> _context = new();

//         private readonly Data.Repositories.Application.ApplicationPageRepository _sut;

//         public WhenUpsertingPage() => _sut = new(_context.Object);

//         [Fact]
//         public async Task Then_Page_Is_Upserted()
//         {
//             // Arrange
//             Guid applicationId = Guid.NewGuid();
//             Guid pageId = Guid.NewGuid();

//             ApplicationPage page = new()
//             {
//                 ApplicationId = applicationId,
//                 PageId = pageId
//             };

//             var dbSet = new List<ApplicationPage>() { page };

//             _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

//             // Act
//             // var result = await _sut.GetApplicationPageByPageIdAsync(applicationId, pageId);

//             // Assert
//             Assert.NotNull(result);
//             Assert.Equal(page, result);

//             _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

//             // Act
//             var result = await _sut.UpsertPage(page);

//             // Assert
//             _context.Verify(c => c.ApplicationPages.Update(page), Times.Once());
//             _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
//         }
//         //         public async Task UpsertAsync(List<ApplicationPage> applicationPagesToUpsert)
//         // {
//         //     foreach (var page in applicationPagesToUpsert)
//         //     {
//         //         UpsertPage(page);
//         //     }
//         //     await _context.SaveChangesAsync();
//         // }

//         // public async Task UpsertAsync(ApplicationPage page)
//         // {
//         //     UpsertPage(page);

//         //     await _context.SaveChangesAsync();
//         // }

//     }
// }


