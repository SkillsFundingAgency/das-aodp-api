using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Tests.Repositories.FormBuilder.FormRepository;

public class WhenMovingFormOrderUp
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.FormBuilder.FormRepository _sut;

    public WhenMovingFormOrderUp() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Move_Form_Order_Up()
    {
        // Arrange
        Guid formId1 = Guid.NewGuid();

        Guid formId2 = Guid.NewGuid(); 

        Form newForm1 = new()
        {
            Id = formId1,
            Order = 0      
        };

        Form newForm2 = new()
        {
            Id = formId2,
            Order = 1
        };
        
        var dbSet = new List<Form>() { newForm1, newForm2 };

        _context.SetupGet(c => c.Forms).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.MoveFormOrderUp(formId1);

        // Assert
        Assert.True(result);
    }
}
