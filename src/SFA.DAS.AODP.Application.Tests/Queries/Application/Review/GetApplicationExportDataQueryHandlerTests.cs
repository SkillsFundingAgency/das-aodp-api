using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Moq;
using SFA.DAS.AODP.Application.Queries.Application.Application;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Shared.UnitTests.Helpers;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Review
{
    public class GetApplicationExportDataQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationRepository> _repositoryMock;
        private readonly GetApplicationExportDataQueryHandler _handler;

        public GetApplicationExportDataQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _repositoryMock = _fixture.Freeze<Mock<IApplicationRepository>>();

            _handler = new GetApplicationExportDataQueryHandler(
                _mediatorMock.Object,
                _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenAllDependenciesSucceed()
        {
            // Arrange
            var applicationReviewId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();

            var formAnswersResponse = _fixture.Create<GetApplicationFormAnswersByReviewIdQueryResponse>();
            formAnswersResponse.ApplicationId = applicationId;

            var formPreviewResponse = _fixture.Create<GetApplicationFormPreviewByIdQueryResponse>();

            var applicationSummary = _fixture.Create<ApplicationExportMetadata>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormAnswersByReviewIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BaseMediatrResponse<GetApplicationFormAnswersByReviewIdQueryResponse>
                {
                    Value = formAnswersResponse,
                    Success = true
                });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormPreviewByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>
                {
                    Value = formPreviewResponse,
                    Success = true
                });

            _repositoryMock
                .Setup(r => r.GetApplicationExportMetadataAsync(applicationId))
                .ReturnsAsync(applicationSummary);

            var request = new GetApplicationExportDataQuery(applicationReviewId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);

            Assert.Equal(formAnswersResponse, result.Value.ApplicationFormResponse);
            Assert.Equal(formPreviewResponse, result.Value.ApplicationFormStructure);
            Assert.NotNull(result.Value.ApplicationMetadata);

            _mediatorMock.Verify(m =>
                m.Send(It.Is<GetApplicationFormAnswersByReviewIdQuery>(q =>
                    q.ApplicationReviewId == applicationReviewId),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(m =>
                m.Send(It.Is<GetApplicationFormPreviewByIdQuery>(q =>
                    q.ApplicationId == applicationId),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _repositoryMock.Verify(r =>
                r.GetApplicationExportMetadataAsync(applicationId),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenExceptionThrown()
        {
            // Arrange
            var applicationReviewId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormAnswersByReviewIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            var request = new GetApplicationExportDataQuery(applicationReviewId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Mediator failure", result.ErrorMessage);
            Assert.NotNull(result.InnerException);
        }
    }
}

