// using Moq;
// using Moq.EntityFrameworkCore;
// using SFA.DAS.AODP.Data.Context;
// using SFA.DAS.AODP.Data.Entities.Application;

// namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationQuestionAnswerRepository
// {
//     public class WhenUpsertingApplicationQuestionAnswer
//     {
//         private readonly Mock<IApplicationDbContext> _context = new();

//         private readonly Data.Repositories.Application.ApplicationQuestionAnswerRepository _sut;

//         public WhenUpsertingApplicationQuestionAnswer() => _sut = new(_context.Object);

//         [Fact]
//         public async Task Then_ApplicationQuestionAnswer_Is_Upserted()
//         {
//             // Arrange
//             Guid applicationId = Guid.NewGuid();
//             Guid pageId = Guid.NewGuid();

//             ApplicationQuestionAnswer questionAnswer = new()
//             {
//                 Id = applicationId,
//                 ApplicationPageId = pageId
//             };

//             var dbSet = new List<ApplicationQuestionAnswer>() { questionAnswer };

//             _context.SetupGet(c => c.ApplicationQuestionAnswers).ReturnsDbSet(dbSet);

//             // // Act
//             // // var result = await _sut.GetApplicationPageByPageIdAsync(applicationId, pageId);

//             // // Assert
//             // Assert.NotNull(result);
//             // Assert.Equal(page, result);

//             // _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

//             // Act
//             var result = await _sut.UpsertAsync();

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


