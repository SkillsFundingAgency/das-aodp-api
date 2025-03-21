﻿using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationPageRepository;
public class WhenSkippingApplicationPagesByQuestionId
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationPageRepository _sut;

    public WhenSkippingApplicationPagesByQuestionId() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Skip_ApplicationPages_By_Question_Id()
    {
        // Arrange
        Guid applicationId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();
        List<Guid> pageIdsToIgnore = new List<Guid>{
            Guid.NewGuid()
        };

        ApplicationPage page = new()
        {
            ApplicationId = applicationId,
            SkippedByQuestionId = questionId
        };

        var dbSet = new List<ApplicationPage>() { page };

        _context.SetupGet(c => c.ApplicationPages).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetSkippedApplicationPagesByQuestionIdAsync(applicationId, questionId, pageIdsToIgnore);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dbSet, result);
    }
}



