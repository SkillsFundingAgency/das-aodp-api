using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetQualificationDetailsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<INewQualificationsRepository> _repositoryMock;
        private readonly Mock<IQualificationDetailsRepository> _detailsRepositoryMock;
        private readonly GetQualificationDetailsQueryHandler _handler;

        public GetQualificationDetailsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<INewQualificationsRepository>>();
            _detailsRepositoryMock = _fixture.Freeze<Mock<IQualificationDetailsRepository>>();
            _handler = _fixture.Create<GetQualificationDetailsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_QualificationDetailsData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            var qual = _fixture.Create<QualificationVersions>();
            response.Success = true;

            _detailsRepositoryMock.Setup(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(qual);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _detailsRepositoryMock.Verify(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Value.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Failure_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailsQuery>();
            var baseResponse = new BaseMediatrResponse<GetQualificationDetailsQueryResponse>
            {
                Success = false,
                Value = null,
                ErrorMessage = $"No details found for qualification reference: {query.QualificationReference}"
            };

            _detailsRepositoryMock.Setup(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()))
                           .Throws(new RecordWithNameNotFoundException(""));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _detailsRepositoryMock.Verify(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal($"No details found for qualification reference: {query.QualificationReference}", result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationDetailsQuery>();
            var exceptionMessage = "An error occurred";
            _detailsRepositoryMock.Setup(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _detailsRepositoryMock.Verify(x => x.GetQualificationDetailsByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }
    }
}


