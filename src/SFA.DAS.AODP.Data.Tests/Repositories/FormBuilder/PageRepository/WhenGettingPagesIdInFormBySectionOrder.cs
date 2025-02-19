using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenGettingPagesIdInFormBySectionOrder
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenGettingPagesIdInFormBySectionOrder() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Pages_Id_In_Form_By_Section_Order()
    {
        // Arrange
        Guid formId = Guid.NewGuid();
        Guid formVersionId = Guid.NewGuid();
        Guid sectionId1 = Guid.NewGuid();
        Guid sectionId2 = Guid.NewGuid();
        Guid sectionId3 = Guid.NewGuid();
        Guid pageId1 = Guid.NewGuid();
        Guid pageId2 = Guid.NewGuid();
        Guid pageId3 = Guid.NewGuid();

        Page newPage1 = new()
        {
            Id = pageId1,
            Order = 0
        };

        Page newPage2 = new()
        {
            Id = pageId2,
            Order = 0
        };

        Page newPage3 = new()
        {
            Id = pageId2,
            Order = 0
        };

        Section newSection1 = new()
        {
            Id = sectionId1,
            FormVersionId = formId,
            Pages = [
                newPage1
            ]
        };

        Section newSection2 = new()
        {
            Id = sectionId2,
            FormVersionId = formId,
            Pages = [
                newPage2
            ]
        };

        Section newSection3 = new()
        {
            Id = sectionId3,
            FormVersionId = formId,
            Pages = [
                newPage3
            ]
        };

        Form newForm = new()
        {
            Id = formId,
            Versions = [
                new FormVersion()
                {
                    Id = formVersionId,
                    FormId = formId,
                    Sections = [
                        newSection1,
                        newSection2,
                        newSection3
                    ]
                }
            ]
        };

        var expectedAnswer = new List<Guid>() { pageId1, pageId2 };

        var dbSet = new List<Form>() { newForm };

        _context.SetupGet(c => c.Forms).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetPagesIdInFormBySectionOrderAsync(formId, 0, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAnswer, result);
    }
}
