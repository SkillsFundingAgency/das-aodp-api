using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SFA.DAS.AODP.Data.Entities.Qualification;

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
            UserType userType = UserType.AwardingOrganisation;

            //SkillsEngland,
        //Ofqual,
        //Qfau

            int offset = 0;

            int limit = 10;

            bool includeApplicationWithNewMessages = false;

            List<string> applicationStatuses = new()
            {
                " "
            };

            Guid applicationId = Guid.NewGuid();

            Guid applicationReviewId = Guid.NewGuid();

            string applicationSearch = " ";

            string awardingOrganisationSearch = " ";

            var query = new GetApplicationsForReviewQuery()
            {
                ReviewUser = UserType.AwardingOrganisation.ToString(),
                ApplicationsWithNewMessages = true
            };
            (List<ApplicationReviewFeedback>, int) response = (new()
            {
                new()
                {
                    Owner = " ",
                    Id = applicationId,
                    ApplicationReviewId = applicationReviewId,
                    Status = " ",
                    NewMessage = true,
                    Type = " ",
                    ApplicationReview = new()
                    {
                        Application = new()
                        {
                            Id = applicationId,
                            Name = " ",
                            ReferenceId = 1,
                            UpdatedAt = DateTime.UtcNow,
                            QualificationNumber = "1",
                            AwardingOrganisationName = "SkillsEngland"

                        }
                    }
                }
            }, 1);

            _repositoryMock.Setup(x => x.GetApplicationReviews(userType, offset, limit, includeApplicationWithNewMessages, applicationStatuses, applicationSearch, awardingOrganisationSearch))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetApplicationReviews(It.IsAny<UserType>(), offset, limit, includeApplicationWithNewMessages, applicationStatuses, applicationSearch, awardingOrganisationSearch), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Item1.Count, result.Value.Applications.Count);
        }

        //[Fact]
        //public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        //{
        //    // Arrange
        //    Guid applicationReviewId = Guid.NewGuid();

        //    Exception ex = new Exception();

        //    var query = new GetApplicationsForReviewQuery();
        //    var response = new ApplicationReview()
        //    {
        //        Id = applicationReviewId,
        //        ApplicationReviewFeedbacks = new()
        //        {
        //            new()
        //        }
        //    };

        //    _repositoryMock.Setup(x => x.GetApplicationReviews(applicationReviewId))
        //                   .ThrowsAsync(ex);

        //    // Act
        //    var result = await _handler.Handle(query, CancellationToken.None);

        //    // Assert
        //    _repositoryMock.Verify(x => x.GetApplicationReviews(applicationReviewId), Times.Once);
        //    Assert.False(result.Success);
        //    Assert.Equal(ex.Message, result.ErrorMessage);
        //}
    }
}