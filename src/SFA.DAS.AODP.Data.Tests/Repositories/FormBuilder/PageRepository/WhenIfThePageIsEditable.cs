using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.PageRepository;

public class WhenPageIsEditable
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.PageRepository _sut;

    public WhenPageIsEditable() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_If_The_Page_Is_Editable()
    {  
        // Arrange
        Guid pageId = Guid.NewGuid();

        Page page = new()
        {
            Id = pageId,
            Section = new()
            {
                FormVersion = new()
                {
                    Status = FormVersionStatus.Draft.ToString()
                }
            }
        };

        var dbSet = new List<Page>() { page };

        _context.SetupGet(c => c.Pages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.IsPageEditable(pageId);

        // Assert
        Assert.True(result);
    }
}