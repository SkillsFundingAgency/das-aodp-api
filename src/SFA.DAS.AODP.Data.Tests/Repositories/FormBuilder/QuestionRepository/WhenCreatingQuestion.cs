using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.QuestionRepository;

public class WhenCreatingQuestion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.QuestionRepository _sut;

    public WhenCreatingQuestion() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Create_Question()
    {
        // Arrange
        Question newQuestion = new() { PageId = Guid.NewGuid() };
        Page newPage = new() 
        { 
            Id = newQuestion.PageId,
            Section = new()
            {
                FormVersion = new()
                {
                    Status = FormVersionStatus.Draft.ToString()
                }
            }
        };
        var dbSet = new List<Question>() { newQuestion };
        var pages = new List<Page>() { newPage };

        _context.SetupGet(c => c.Questions).ReturnsDbSet(dbSet);
        _context.SetupGet(c => c.Pages).ReturnsDbSet(pages);

        // Act
        var result = await _sut.Create(newQuestion);

        // Assert
        _context.Verify(c => c.Questions.Add(newQuestion), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);
    }
}
