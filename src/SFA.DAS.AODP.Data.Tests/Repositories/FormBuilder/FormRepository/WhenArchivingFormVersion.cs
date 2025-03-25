using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Models.Form;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.FormRepository;

public class WhenArchivingForm
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.FormRepository _sut;

    public WhenArchivingForm() => _sut = new(
        _context.Object
    );

    [Fact]
    public async Task When_The_Form_Is_Archived()
    {
        // Arrange
        Guid formId = Guid.NewGuid();

        Form form = new()
        {
            Id = formId,
        };

        var dbSet = new List<Form>() { form };

        _context.SetupGet(c => c.Forms).ReturnsDbSet(dbSet);

        // Act
        await _sut.Archive(formId);

        // Assert
        Assert.Equal(FormStatus.Deleted.ToString(), form.Status);
        _context.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }
}
