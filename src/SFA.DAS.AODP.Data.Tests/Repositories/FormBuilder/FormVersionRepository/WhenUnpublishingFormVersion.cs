using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.FormVersionRepository;

public class WhenUnpublishingFormVersion
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.FormVersionRepository _sut;

    private readonly Mock<ISectionRepository> _sectionRepository = new();
    private readonly Mock<IPageRepository> _pageRepository = new();
    private readonly Mock<IQuestionRepository> _questionRepository = new();
    private readonly Mock<IQuestionOptionRepository> _questionOptionRepository = new();
    private readonly Mock<IQuestionValidationRepository> _questionValidationRepository = new();
    private readonly Mock<IRouteRepository> _routeRepository = new();
    public WhenUnpublishingFormVersion() => _sut = new(
        _context.Object, 
        _sectionRepository.Object, 
        _pageRepository.Object, 
        _questionRepository.Object,
        _questionOptionRepository.Object,
        _questionValidationRepository.Object,
        _routeRepository.Object
    );
    [Fact]
    public async Task Then_Unpublish_FormVersion()
    {
        // Arrange
        Guid formId = Guid.NewGuid();

        FormVersion form = new()
        {
            Id = formId,
        };

        var dbSet = new List<FormVersion>() { form };

        _context.SetupGet(c => c.FormVersions).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.Unpublish(formId);

        // Assert
        Assert.True(result);
    }
}

