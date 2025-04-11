using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using System;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Review
{
    public class GetFeedbackForApplicationReviewByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationReviewFeedbackRepository> _repositoryMock;
        private readonly GetFeedbackForApplicationReviewByIdQueryHandler _handler;
        public GetFeedbackForApplicationReviewByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationReviewFeedbackRepository>>();
            _handler = _fixture.Create<GetFeedbackForApplicationReviewByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_FeedbackForApplicationReview_Is_Returned()
        {
            // Arrange
            Guid applicationReviewId = Guid.NewGuid();

            UserType userType = UserType.AwardingOrganisation;

            string userTypeString = userType.ToString();

            var query = new GetFeedbackForApplicationReviewByIdQuery(applicationReviewId, userTypeString);
            var response = new ApplicationReviewFeedback()
            {
                Id = applicationReviewId,
                Type = userTypeString,
                ApplicationReview = new()
                {
                    ApplicationId = Guid.NewGuid()
                }
            };

            _repositoryMock.Setup(x => x.GeyByReviewIdAndUserType(applicationReviewId, userType))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GeyByReviewIdAndUserType(applicationReviewId, userType), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Type, result.Value.UserType);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid applicationReviewId = Guid.NewGuid();

            UserType userType = UserType.AwardingOrganisation;

            string userTypeString = userType.ToString();

            var query = new GetFeedbackForApplicationReviewByIdQuery(applicationReviewId, userTypeString);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GeyByReviewIdAndUserType(applicationReviewId, userType))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GeyByReviewIdAndUserType(applicationReviewId, userType), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}