﻿using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetNewQualificationsCSVExportQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<INewQualificationsRepository> _repositoryMock;
        private readonly GetNewQualificationsCSVExportHandler _handler;
        public GetNewQualificationsCSVExportQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<INewQualificationsRepository>>();
            _handler = _fixture.Create<GetNewQualificationsCSVExportHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_QualificationExports_Are_Returned()
        {
            // Arrange
            var query = new GetNewQualificationsCsvExportQuery();
            var response = new List<NewQualificationExport>()
            {
                new NewQualificationExport()
                {
                    DateOfDownload = DateTime.UtcNow
                }            };

            _repositoryMock.Setup(x => x.GetNewQualificationsExport())
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetNewQualificationsExport(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.QualificationExports.Count());
            Assert.Single(result.Value.QualificationExports);
            Assert.Equal(response.First().DateOfDownload, result.Value.QualificationExports.First().DateOfDownload);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_No_QualificationExports_Are_Returned()
        {
            // Arrange
            var query = new GetNewQualificationsCsvExportQuery();
            List<NewQualificationExport> response = null;

            _repositoryMock.Setup(x => x.GetNewQualificationsExport())
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetNewQualificationsExport(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("No new qualifications found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = new GetNewQualificationsCsvExportQuery();

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetNewQualificationsExport())
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetNewQualificationsExport(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}