using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetNewQualificationsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationsRepository> _repositoryMock;
        private readonly GetNewQualificationsQueryHandler _handler;

        public GetNewQualificationsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IQualificationsRepository>>();
            _handler = _fixture.Create<GetNewQualificationsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NewQualificationsData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetNewQualificationsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            response.Success = true;
            response.Value.NewQualifications = _fixture.CreateMany<NewQualification>(2).ToList();

            _repositoryMock.Setup(x => x.GetAllNewQualificationsAsync())
                           .ReturnsAsync(response.Value.NewQualifications);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllNewQualificationsAsync(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(2, result.Value.NewQualifications.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Failure_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetNewQualificationsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            response.Success = false;
            response.Value = null;

            _repositoryMock.Setup(x => x.GetAllNewQualificationsAsync())
                           .ReturnsAsync(new List<NewQualification>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllNewQualificationsAsync(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("No new qualifications found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = _fixture.Create<GetNewQualificationsQuery>();
            var exceptionMessage = "An error occurred";
            _repositoryMock.Setup(x => x.GetAllNewQualificationsAsync())
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllNewQualificationsAsync(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }
    }
}


