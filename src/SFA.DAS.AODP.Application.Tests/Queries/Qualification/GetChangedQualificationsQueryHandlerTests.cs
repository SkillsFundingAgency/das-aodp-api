using Moq;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Application.Queries.Qualification;
using AutoFixture;
using AutoFixture.AutoMoq;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Tests.Queries.Qualification
{
    public class GetChangedQualificationsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IChangedQualificationsRepository> _repositoryMock;
        private readonly GetChangedQualificationsQueryHandler _handler;
        public GetChangedQualificationsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IChangedQualificationsRepository>>();
            _handler = _fixture.Create<GetChangedQualificationsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NewQualificationsData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetChangedQualificationsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetChangedQualificationsQueryResponse>>();
            response.Success = true;
            response.Value = new GetChangedQualificationsQueryResponse()
            {
                TotalRecords = 2,
                Data = _fixture.CreateMany<ChangedQualification>(2).ToList(),
                Skip = 10,
                Take = 20
            };

            _repositoryMock.Setup(x => x.GetAllChangedQualificationsAsync(query.Skip, query.Take, It.IsAny<QualificationsFilter>()))
                           .ReturnsAsync(new ChangedQualificationsResult() { Data = response.Value.Data, TotalRecords = response.Value.TotalRecords });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllChangedQualificationsAsync(query.Skip, query.Take, It.IsAny<QualificationsFilter>()), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(2, result.Value.Data.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Empty_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetChangedQualificationsQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetChangedQualificationsQueryResponse>>();
            response.Success = false;
            response.Value = null;

            _repositoryMock.Setup(x => x.GetAllChangedQualificationsAsync(query.Skip, query.Take, It.IsAny<QualificationsFilter>()))
                           .ReturnsAsync(new ChangedQualificationsResult());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllChangedQualificationsAsync(query.Skip, query.Take, It.IsAny<QualificationsFilter>()), Times.Once);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = _fixture.Create<GetChangedQualificationsQuery>();
            var exceptionMessage = "An error occurred";
            _repositoryMock.Setup(x => x.GetAllChangedQualificationsAsync(query.Skip, query.Take, It.IsAny<QualificationsFilter>()))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllChangedQualificationsAsync(query.Skip, query.Take, It.IsAny<QualificationsFilter>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }
    }
}


