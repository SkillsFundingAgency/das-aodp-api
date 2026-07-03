using AutoFixture;
using AutoFixture.AutoMoq;
using Azure;
using Moq;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Review
{
    public class GetApplicationsForReviewQueryResponseTests
    {
        [Fact]
        public void Map_Should_Map_Single_Review_Correctly()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            var reviews = new List<ApplicationReviewFeedback>
            {
                new()
                {
                    ApplicationReviewId = reviewId,
                    Status = "submitted",
                    NewMessage = true,
                    Owner = "owner-1",
                    ApplicationReview = new()
                    {
                        Application = new()
                        {
                            Id = applicationId,
                            Name = "Test App",
                            UpdatedAt = DateTime.UtcNow,
                            ReferenceId = 123,
                            QualificationNumber = "QAN1",
                            AwardingOrganisationName = "Org1",
                            Reviewer1 = "Bob",
                            Reviewer2 = "Alice",
                            SubmittedAt = DateTime.UtcNow.AddDays(-1)
                        }
                    }
                }
            };

            // Act
            var result = GetApplicationsForReviewQueryResponse.Map(reviews, 5);

            // Assert
            Assert.Equal(5, result.TotalRecordsCount);
            Assert.Single(result.Applications);

            var app = result.Applications.First();

            Assert.Equal(applicationId, app.Id);
            Assert.Equal(reviewId, app.ApplicationReviewId);
            Assert.Equal("Test App", app.Name);
            Assert.Equal("submitted", app.Status);
            Assert.True(app.NewMessage);
            Assert.Equal("owner-1", app.Owner);
            Assert.Equal("Org1", app.AwardingOrganisation);
            Assert.Equal("Bob", app.Reviewer1);
            Assert.Equal("Alice", app.Reviewer2);
        }

        [Fact]
        public void Map_Should_Map_Multiple_Reviews()
        {
            // Arrange
            var reviews = new List<ApplicationReviewFeedback>
            {
                CreateReview("App1"),
                CreateReview("App2"),
                CreateReview("App3")
            };

            // Act
            var result = GetApplicationsForReviewQueryResponse.Map(reviews, 3);

            // Assert
            Assert.Equal(3, result.Applications.Count);
            Assert.Equal("App1", result.Applications[0].Name);
            Assert.Equal("App2", result.Applications[1].Name);
            Assert.Equal("App3", result.Applications[2].Name);
        }

        [Fact]
        public void Map_Should_Return_Empty_List_When_No_Reviews()
        {
            // Act
            var result = GetApplicationsForReviewQueryResponse.Map(new List<ApplicationReviewFeedback>(), 0);

            // Assert
            Assert.Empty(result.Applications);
            Assert.Equal(0, result.TotalRecordsCount);
        }

        [Fact]
        public void Map_Should_Throw_When_Nested_Application_Is_Null()
        {
            // Arrange
            var reviews = new List<ApplicationReviewFeedback>
            {
                new()
                {
                    ApplicationReview = new()
                    {
                        Application = null!
                    }
                }
            };

            // Act + Assert
            Assert.Throws<NullReferenceException>(() =>
                GetApplicationsForReviewQueryResponse.Map(reviews, 1));
        }


        private static ApplicationReviewFeedback CreateReview(string name)
        {
            return new ApplicationReviewFeedback
            {
                Status = "submitted",
                NewMessage = false,
                ApplicationReview = new()
                {
                    Application = new()
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        UpdatedAt = DateTime.UtcNow,
                        ReferenceId = 1
                    }
                }
            };
        }
    }
}