using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Models.Application;
using System;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationReviewFeedback;

public class WhenGettingReviewDetails
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicationDbContext> _context = new();
    private readonly Data.Repositories.Application.ApplicationReviewFeedbackRepository _sut;
    public WhenGettingReviewDetails() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Application_Review_By_Id()
    {
        // Arrange
        var feedback = new Entities.Application.ApplicationReviewFeedback()
        {
            Id = Guid.NewGuid(),
            ApplicationReviewId = Guid.NewGuid(),
            Type = UserType.Qfau.ToString()
        };

        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback]);


        // Act
        var result = await _sut.GetApplicationReviewFeedbackDetailsByReviewIdAsync(feedback.ApplicationReviewId, UserType.Qfau);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(feedback, result);
    }
}


