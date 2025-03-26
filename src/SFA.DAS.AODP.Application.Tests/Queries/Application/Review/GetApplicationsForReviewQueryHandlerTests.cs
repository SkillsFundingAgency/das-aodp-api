using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Review
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
                            AwardingOrganisationName = "SkillsEngland"

                        }
                    }
                }
            }, 1);

            _repositoryMock.Setup(x => x.GetApplicationReviews(
                UserType.Qfau, 
                query.Offset.Value,
                query.Limit.Value,
                query.ApplicationsWithNewMessages,
                query.ApplicationStatuses,
                query.ApplicationSearch,
                query.AwardingOrganisationSearch))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);

            Assert.Equal(response.Item2, result.Value.TotalRecordsCount);

            Assert.Single(result.Value.Applications);
            Assert.Equal(response.Item1.First().ApplicationReview.Application.Id, result.Value.Applications.First().Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            // Arrange
            var query = new GetApplicationsForReviewQuery()
            {
                ReviewUser = UserType.Qfau.ToString(),
                ApplicationsWithNewMessages = true,
                AwardingOrganisationSearch = _fixture.Create<string>(),
                ApplicationSearch = _fixture.Create<string>(),
                ApplicationStatuses = _fixture.CreateMany<string>().ToList(),
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
                            AwardingOrganisationName = "SkillsEngland"

                        }
                    }
                }
            }, 1);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetApplicationReviews(
                UserType.Qfau,
                query.Offset.Value,
                query.Limit.Value,
                query.ApplicationsWithNewMessages,
                query.ApplicationStatuses,
                query.ApplicationSearch,
                query.AwardingOrganisationSearch))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}