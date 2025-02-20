using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.SectionRepository;

public class WhenGettingMaxOrderByFormVersionId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.SectionRepository _sut;

    private readonly Mock<IPageRepository> _pageRepository = new();

    public WhenGettingMaxOrderByFormVersionId() => _sut = new(
        _context.Object,
        _pageRepository.Object
    );
    [Fact]
    public async Task Then_Get_Max_Order_By_Form_Version_Id()
    {
        // Arrange
        Guid formVersionId = Guid.NewGuid();
        Guid sectionId = Guid.NewGuid();

        Section newSection = new()
        {
            Id = sectionId,
            FormVersionId = formVersionId,
            Order = 0,
        };

        var dbSet = new List<Section>() { newSection };

        _context.SetupGet(c => c.Sections).ReturnsDbSet(dbSet);

        // Act
        var result = _sut.GetMaxOrderByFormVersionId(formVersionId);

        // Assert
        Assert.Equal(newSection.Order, result);
    }
}
