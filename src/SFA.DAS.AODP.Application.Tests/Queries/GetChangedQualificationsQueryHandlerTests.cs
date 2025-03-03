using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetChangedQualificationsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationsRepository> _repositoryMock;
        private readonly GetChangedQualificationsQueryHandler _handler;

        public GetChangedQualificationsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IQualificationsRepository>>();
            _handler = _fixture.Create<GetChangedQualificationsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ChangedQualificationsData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetChangedQualificationsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetChangedQualificationsQueryResponse>>();
            response.Success = true;
            response.Value.ChangedQualifications = _fixture.CreateMany<ChangedQualification>(2).ToList();

            _repositoryMock.Setup(x => x.GetAllChangedQualificationsAsync())
                           .ReturnsAsync(response.Value.ChangedQualifications);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllChangedQualificationsAsync(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(2, result.Value.ChangedQualifications.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Failure_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetChangedQualificationsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetChangedQualificationsQueryResponse>>();
            response.Success = false;
            response.Value = null;

            _repositoryMock.Setup(x => x.GetAllChangedQualificationsAsync())
                           .ReturnsAsync(new List<ChangedQualification>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllChangedQualificationsAsync(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("No new qualifications found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = _fixture.Create<GetChangedQualificationsQuery>();
            var exceptionMessage = "An error occurred";
            _repositoryMock.Setup(x => x.GetAllChangedQualificationsAsync())
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllChangedQualificationsAsync(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }
    }
}


