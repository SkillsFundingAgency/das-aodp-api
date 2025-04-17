using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetQualificationDetailWithVersionsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IChangedQualificationsRepository> _repositoryMock;
        private readonly Mock<IQualificationDetailsRepository> _detailsRepositoryMock;
        private readonly GetQualificationDetailWithVersionsQueryHandler _handler;

        public GetQualificationDetailWithVersionsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repositoryMock = _fixture.Freeze<Mock<IChangedQualificationsRepository>>();
            _detailsRepositoryMock = _fixture.Freeze<Mock<IQualificationDetailsRepository>>();
            _handler = _fixture.Create<GetQualificationDetailWithVersionsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_QualificationDetailsData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailWithVersionsQuery>();
            var qual = _fixture.Create<QualificationVersions>();

            _detailsRepositoryMock.Setup(x => x.GetQualificationDetailWithVersions(It.IsAny<string>()))
                           .ReturnsAsync(qual);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _detailsRepositoryMock.Verify(x => x.GetQualificationDetailWithVersions(It.IsAny<string>()), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(qual.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Failure_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailWithVersionsQuery>();
            var baseResponse = new BaseMediatrResponse<GetQualificationDetailsQueryResponse>
            {
                Success = false,
                Value = null,
                ErrorMessage = $"No details found for qualification reference: {query.QualificationReference}"
            };

            _detailsRepositoryMock.Setup(x => x.GetQualificationDetailWithVersions(It.IsAny<string>()))
                           .Throws(new RecordWithNameNotFoundException(""));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _detailsRepositoryMock.Verify(x => x.GetQualificationDetailWithVersions(It.IsAny<string>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal($"No details found for qualification reference: {query.QualificationReference}", result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailWithVersionsQuery>();
            var exceptionMessage = "An error occurred";
            _detailsRepositoryMock.Setup(x => x.GetQualificationDetailWithVersions(It.IsAny<string>()))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _detailsRepositoryMock.Verify(x => x.GetQualificationDetailWithVersions(It.IsAny<string>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }
    }
}


