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
    public class GetApplicationsForReviewQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationReviewFeedbackRepository> _repositoryMock;
        private readonly GetApplicationsForReviewQueryHandler _handler;
        public GetApplicationsForReviewQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationReviewFeedbackRepository>>();
            _handler = _fixture.Create<GetApplicationsForReviewQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationReviews_Are_Returned()
        {
            // Arrange
            var query = new GetApplicationsForReviewQuery()
            {
                ReviewUser = UserType.Qfau.ToString(),
                ApplicationsWithNewMessages = true,
                AwardingOrganisationSearch = _fixture.Create<string>(),
                ApplicationSearch = _fixture.Create<string>(),
                ApplicationStatuses = _fixture.CreateMany<string>().ToList(),
                UnassignedOnly = false,
                ReviewerSearch = _fixture.Create<string>(), 
                Limit = 1,
                Offset = 1
            };

            (List<ApplicationReviewFeedback>, int) response = (new()
            {
                new()
                {
                    Owner = " ",
                    Id = Guid.NewGuid(),
                    ApplicationReviewId = Guid.NewGuid(),
                    Status = " ",
                    NewMessage = true,
                    Type = " ",
                    ApplicationReview = new()
                    {
                        Application = new()
                        {
                            Id = Guid.NewGuid(),
                            Name = " ",
                            ReferenceId = 1,
                            UpdatedAt = DateTime.UtcNow,
                            QualificationNumber = "1",
                            AwardingOrganisationName = "SkillsEngland",
                            Reviewer1 = "Bob Smith",
                            Reviewer2 = "Sam Jones"

                        }
                    }
                }
            }, 1);

            _repositoryMock
                .Setup(x => x.GetApplicationReviews(It.Is<ApplicationReviewSearchCriteria>(c =>
                    c.ReviewType == UserType.Qfau &&
                    c.Offset == query.Offset!.Value &&
                    c.Limit == query.Limit!.Value &&
                    c.IncludeApplicationWithNewMessages == query.ApplicationsWithNewMessages &&
                    c.UnassignedOnly == query.UnassignedOnly &&
                    c.ApplicationSearch == query.ApplicationSearch &&
                    c.AwardingOrganisationSearch == query.AwardingOrganisationSearch &&
                    c.ReviewerSearch == query.ReviewerSearch &&
                    c.ApplicationStatuses != null &&
                    c.ApplicationStatuses.SequenceEqual(query.ApplicationStatuses)
                )))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);

            Assert.Equal(response.Item2, result.Value.TotalRecordsCount);

            Assert.Single(result.Value.Applications);
            Assert.Equal(response.Item1.First().ApplicationReview.Application.Id, result.Value.Applications.First().Id);

            var returned = result.Value.Applications.First();
            Assert.Equal("Bob Smith", returned.Reviewer1);
            Assert.Equal("Sam Jones", returned.Reviewer2);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = new GetApplicationsForReviewQuery()
            {
                ReviewUser = UserType.Qfau.ToString(),
                ApplicationsWithNewMessages = true,
                AwardingOrganisationSearch = _fixture.Create<string>(),
                ApplicationSearch = _fixture.Create<string>(),
                ApplicationStatuses = _fixture.CreateMany<string>().ToList(),
                UnassignedOnly = false,
                ReviewerSearch = _fixture.Create<string>(),
                Limit = 1,
                Offset = 1
            };

            (List<ApplicationReviewFeedback>, int) response = (new()
            {
                new()
                {
                    Owner = " ",
                    Id = Guid.NewGuid(),
                    ApplicationReviewId = Guid.NewGuid(),
                    Status = " ",
                    NewMessage = true,
                    Type = " ",
                    ApplicationReview = new()
                    {
                        Application = new()
                        {
                            Id = Guid.NewGuid(),
                            Name = " ",
                            ReferenceId = 1,
                            UpdatedAt = DateTime.UtcNow,
                            QualificationNumber = "1",
                            AwardingOrganisationName = "SkillsEngland",
                            Reviewer1 = "Alice Smith",
                            Reviewer2 = "Sam Jones"

                        }
                    }
                }
            }, 1);

            Exception ex = new Exception();

            _repositoryMock
                .Setup(x => x.GetApplicationReviews(It.Is<ApplicationReviewSearchCriteria>(c =>
                    c.ReviewType == UserType.Qfau &&
                    c.Offset == query.Offset!.Value &&
                    c.Limit == query.Limit!.Value &&
                    c.IncludeApplicationWithNewMessages == query.ApplicationsWithNewMessages &&
                    c.UnassignedOnly == query.UnassignedOnly &&
                    c.ApplicationSearch == query.ApplicationSearch &&
                    c.AwardingOrganisationSearch == query.AwardingOrganisationSearch &&
                    c.ReviewerSearch == query.ReviewerSearch &&
                    c.ApplicationStatuses != null &&
                    c.ApplicationStatuses.SequenceEqual(query.ApplicationStatuses)
                )))
                .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }

        [Fact]
        public async Task Then_UnassignedOnly_Is_Passed_To_Repository()
        {
            var query = new GetApplicationsForReviewQuery
            {
                ReviewUser = UserType.Qfau.ToString(),
                UnassignedOnly = true,
                ReviewerSearch = "Bob",
                Limit = 10,
                Offset = 0
            };

            var criteria = new ApplicationReviewSearchCriteria
            {
                ReviewType = UserType.Qfau,
                Offset = query.Offset.Value,
                Limit = query.Limit.Value,
                ApplicationSearch = query.ApplicationSearch,
                ApplicationStatuses = query.ApplicationStatuses,
                AwardingOrganisationSearch = query.AwardingOrganisationSearch,
                ReviewerSearch = query.ReviewerSearch,
                UnassignedOnly = query.UnassignedOnly,
                IncludeApplicationWithNewMessages = query.ApplicationsWithNewMessages,
            };

            _repositoryMock
                .Setup(x => x.GetApplicationReviews(It.Is<ApplicationReviewSearchCriteria>(c =>
                    c.ReviewType == UserType.Qfau &&
                    c.Offset == query.Offset!.Value &&
                    c.Limit == query.Limit!.Value &&
                    c.IncludeApplicationWithNewMessages == query.ApplicationsWithNewMessages &&
                    c.UnassignedOnly == query.UnassignedOnly &&
                    c.ApplicationSearch == query.ApplicationSearch &&
                    c.AwardingOrganisationSearch == query.AwardingOrganisationSearch &&
                    c.ReviewerSearch == query.ReviewerSearch &&
                    c.ApplicationStatuses != null &&
                    c.ApplicationStatuses.SequenceEqual(query.ApplicationStatuses)
                )))
                .ReturnsAsync((new List<ApplicationReviewFeedback>(), 0));

            await _handler.Handle(query, CancellationToken.None);

            _repositoryMock.VerifyAll();
        }

    }
}