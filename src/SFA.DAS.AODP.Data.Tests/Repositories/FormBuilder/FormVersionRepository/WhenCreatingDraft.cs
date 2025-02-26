using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.FormVersionRepository;

public class WhenCreatingDraft
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.FormVersionRepository _sut;

    private readonly Mock<ISectionRepository> _sectionRepository = new();
    private readonly Mock<IPageRepository> _pageRepository = new();
    private readonly Mock<IQuestionRepository> _questionRepository = new();
    private readonly Mock<IQuestionOptionRepository> _questionOptionRepository = new();
    private readonly Mock<IQuestionValidationRepository> _questionValidationRepository = new();
    private readonly Mock<IRouteRepository> _routeRepository = new();
    public WhenCreatingDraft() => _sut = new(
        _context.Object, 
        _sectionRepository.Object, 
        _pageRepository.Object, 
        _questionRepository.Object,
        _questionOptionRepository.Object,
        _questionValidationRepository.Object,
        _routeRepository.Object
    );

    [Fact]
    public async Task When_Creating_Draft_FormVersion()
    {  
        // Arrange
        Guid formId = Guid.NewGuid();

        FormVersion form = new()
        {
            Id = formId,
            Status = FormVersionStatus.Draft.ToString()
        };

        var dbSet = new List<FormVersion>() { form };

        var mockIDbContextTransaction = new Mock<IDbContextTransaction>();
        _context.SetupGet(c => c.FormVersions).ReturnsDbSet(dbSet);
        _context.Setup(c => c.StartTransactionAsync()).Returns(Task.FromResult(mockIDbContextTransaction.Object));

        // Act
        var result = await _sut.CreateDraftAsync(formId);

        // Assert
        _context.Verify(c => c.FormVersions.Add(form), Times.Once());
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());

        Assert.True(result.Id != Guid.Empty);    
    }
}
