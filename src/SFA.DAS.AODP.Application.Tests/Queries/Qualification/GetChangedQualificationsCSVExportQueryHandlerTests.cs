using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetChangedQualificationsCSVExportQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IChangedQualificationsRepository> _repositoryMock;
        private readonly GetChangedQualificationsCSVExportHandler _handler;
        public GetChangedQualificationsCSVExportQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IChangedQualificationsRepository>>();
            _handler = _fixture.Create<GetChangedQualificationsCSVExportHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ChangedExports_Are_Returned()
        {
            // Arrange
            var query = new GetChangedQualificationsCsvExportQuery();
            var response = new List<ChangedQualificationExport>()
            {
                new ChangedQualificationExport()
                {
                    DateOfDownload = DateTime.UtcNow
                }
            };

            _repositoryMock.Setup(x => x.GetChangedQualificationsCSVExport())
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetChangedQualificationsCSVExport(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.QualificationExports.Count());
            Assert.Single(result.Value.QualificationExports);
            Assert.Equal(response.First().DateOfDownload, result.Value.QualificationExports.First().DateOfDownload);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_No_ChangedExports_Are_Returned()
        {
            // Arrange
            var query = new GetChangedQualificationsCsvExportQuery();
            List<ChangedQualificationExport> response = null;

            _repositoryMock.Setup(x => x.GetChangedQualificationsCSVExport())
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetChangedQualificationsCSVExport(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("No changed qualifications found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = new GetChangedQualificationsCsvExportQuery();

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetChangedQualificationsCSVExport())
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetChangedQualificationsCSVExport(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}