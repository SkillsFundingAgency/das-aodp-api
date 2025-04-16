using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Review
{
    public class GetApplicationForReviewByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationReviewRepository> _repositoryMock;
        private readonly GetApplicationForReviewByIdQueryHandler _handler;
        public GetApplicationForReviewByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationReviewRepository>>();
            _handler = _fixture.Create<GetApplicationForReviewByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationReview_Is_Returned()
        {
            // Arrange
            Guid applicationReviewId = Guid.NewGuid();

            var query = new GetApplicationForReviewByIdQuery(applicationReviewId);
            var response = new ApplicationReview()
            {
                Id = applicationReviewId,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                Application = new()
                {
                    Name = " ",
                    Id = Guid.NewGuid(),
                    ReferenceId = 1,
                    QualificationNumber = " ",
                    UpdatedAt = DateTime.UtcNow,
                    AwardingOrganisationName = " ",
                    FormVersion = new()
                    {
                        Title = " "
                    }
                    
                },
                ApplicationReviewFeedbacks = new()
                {
                    new()
                }
            };

            _repositoryMock.Setup(x => x.GetApplicationForReviewByReviewIdAsync(applicationReviewId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetApplicationForReviewByReviewIdAsync(applicationReviewId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.ApplicationReviewFeedbacks.Count, result.Value.Feedbacks.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid applicationReviewId = Guid.NewGuid();

            var query = new GetApplicationForReviewByIdQuery(applicationReviewId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetApplicationForReviewByReviewIdAsync(applicationReviewId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetApplicationForReviewByReviewIdAsync(applicationReviewId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}