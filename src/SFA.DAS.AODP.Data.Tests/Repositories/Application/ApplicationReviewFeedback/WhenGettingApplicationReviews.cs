﻿using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Models.Application;
using System;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationReviewFeedback;

public class WhenGettingApplicationReviews
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicationDbContext> _context = new();
    private readonly Data.Repositories.Application.ApplicationReviewFeedbackRepository _sut;
    public WhenGettingApplicationReviews() => _sut = new(_context.Object);

    [Fact]
    public async Task Then_Get_Applications_With_Count()
    {
        // Arrange
        var application = new Entities.Application.Application()
        {
            Id = Guid.NewGuid(),
        };

        var review = new Entities.Application.ApplicationReview()
        {
            Application = application
        };

        var feedback = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = review,
            Type = UserType.Qfau.ToString()
        };

        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.Qfau, 0, 1, false);

        // Assert
        Assert.NotEmpty(result.Item1);
        Assert.Equal(feedback, result.Item1.First());

        Assert.Equal(1, result.Item2);
    }

    [Fact]
    public async Task With_Offset_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1)
                }
            },
            Type = UserType.Qfau.ToString()
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2)
                }
            },
            Type = UserType.Qfau.ToString()
        };
        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.Qfau, 1, 10, false);

        // Assert
        Assert.Single(result.Item1);
        Assert.Equal(feedback_2, result.Item1.First());

        Assert.Equal(2, result.Item2);
    }
    [Fact]
    public async Task With_Limit_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1)
                }
            },
            Type = UserType.Qfau.ToString()
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2)
                }
            },
            Type = UserType.Qfau.ToString()
        };
        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.Qfau, 0, 1, false);

        // Assert
        Assert.Single(result.Item1);
        Assert.Equal(feedback_1, result.Item1.First());

        Assert.Equal(2, result.Item2);
    }

    [Fact]
    public async Task With_ApplicationSearch_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                    Name = "123",
                    ReferenceId = 456,
                    QualificationNumber = "789"
                }
            },
            Type = UserType.Qfau.ToString()
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                    Name = "456",
                    ReferenceId = 123,
                    QualificationNumber = "789"
                }
            },
            Type = UserType.Qfau.ToString()
        };

        var feedback_3 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2),
                    Name = "789",
                    ReferenceId = 456,
                    QualificationNumber = "123"
                }
            },
            Type = UserType.Qfau.ToString()
        };

        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2, feedback_3]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.Qfau, 0, 10, false, applicationSearch: "123");

        // Assert
        Assert.True(result.Item1.Count == 3);
        Assert.Equal(feedback_1, result.Item1.First());
        Assert.Equal(feedback_2, result.Item1.ElementAt(1));
        Assert.Equal(feedback_3, result.Item1.ElementAt(2));

        Assert.Equal(3, result.Item2);
    }


    [Fact]
    public async Task With_OrganisationSearch_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                    AwardingOrganisationName = "123",
                    AwardingOrganisationUkprn = "456"
                }
            },
            Type = UserType.Qfau.ToString()
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                    AwardingOrganisationName = "456",
                    AwardingOrganisationUkprn = "123"
                }
            },
            Type = UserType.Qfau.ToString()
        };


        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.Qfau, 0, 10, false, awardingOrganisationSearch: "123");

        // Assert
        Assert.True(result.Item1.Count == 2);
        Assert.Equal(feedback_1, result.Item1.First());
        Assert.Equal(feedback_2, result.Item1.ElementAt(1));

        Assert.Equal(2, result.Item2);
    }


    [Fact]
    public async Task With_OfqualUserType_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                },
                SharedWithOfqual = true,
            },
            Type = UserType.Ofqual.ToString()
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                }
            },
            Type = UserType.Qfau.ToString()
        };


        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.Ofqual, 0, 10, false);

        // Assert
        Assert.True(result.Item1.Count == 1);
        Assert.Equal(feedback_1, result.Item1.First());

        Assert.Equal(1, result.Item2);
    }


    [Fact]
    public async Task With_SkillsEnglandUserType_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                },
                SharedWithSkillsEngland = true,
            },
            Type = UserType.SkillsEngland.ToString()
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                }
            },
            Type = UserType.Qfau.ToString()
        };


        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.SkillsEngland, 0, 10, false);

        // Assert
        Assert.True(result.Item1.Count == 1);
        Assert.Equal(feedback_1, result.Item1.First());

        Assert.Equal(1, result.Item2);
    }


    [Fact]
    public async Task With_Status_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                },
                SharedWithSkillsEngland = true,
            },
            Type = UserType.SkillsEngland.ToString(),
            Status = "test"
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                }
            },
            Type = UserType.Qfau.ToString()
        };


        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.SkillsEngland, 0, 10, false, applicationStatuses: ["test"]);

        // Assert
        Assert.True(result.Item1.Count == 1);
        Assert.Equal(feedback_1, result.Item1.First());

        Assert.Equal(1, result.Item2);
    }

    [Fact]
    public async Task With_New_Message_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                },
                SharedWithSkillsEngland = true,
            },
            Type = UserType.SkillsEngland.ToString(),
            NewMessage = true
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                }
            },
            Type = UserType.Qfau.ToString()
        };


        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.SkillsEngland, 0, 10, true);

        // Assert
        Assert.True(result.Item1.Count == 1);
        Assert.Equal(feedback_1, result.Item1.First());

        Assert.Equal(1, result.Item2);
    }
    [Fact]
    public async Task With_Status_And_New_Message_Then_Gets_Applications_With_Count()
    {
        // Arrange
        var feedback_1 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                },
                SharedWithSkillsEngland = true,
            },
            Type = UserType.Qfau.ToString(),
            Status = "test"
        };

        var feedback_2 = new Entities.Application.ApplicationReviewFeedback()
        {
            ApplicationReview = new()
            {
                Application = new()
                {
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1),
                }
            },
            Type = UserType.Qfau.ToString(),
            NewMessage = true
        };


        _context.SetupGet(c => c.ApplicationReviewFeedbacks).ReturnsDbSet([feedback_1, feedback_2]);


        // Act
        var result = await _sut.GetApplicationReviews(UserType.Qfau, 0, 10, true, applicationStatuses: ["test"]);

        // Assert
        Assert.True(result.Item1.Count == 2);
        Assert.Equal(feedback_1, result.Item1.First());
        Assert.Equal(feedback_2, result.Item1.ElementAt(1));

        Assert.Equal(2, result.Item2);
    }
}


