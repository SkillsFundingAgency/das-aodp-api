using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Forms
{
    public class GetAllFormVersionsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFormVersionRepository> _repositoryMock;
        private readonly GetAllFormVersionsQueryHandler _handler;
        public GetAllFormVersionsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IFormVersionRepository>>();
            _handler = _fixture.Create<GetAllFormVersionsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_FormVersions_Are_Returned()
        {
            // Arrange
            var query = new GetAllFormVersionsQuery();
            var response = new List<FormVersion>()
            {
                new FormVersion()
                {
                    Id = Guid.NewGuid(),
                    FormId = Guid.NewGuid(),
                    Title = " ",
                    Version = DateTime.Now,
                    Status = " ",
                    Description = " ",
                    DateCreated = DateTime.Now,
                    Form = new()
                    {
                        Order = 1
                    }
                }
            };

            _repositoryMock.Setup(x => x.GetLatestFormVersions())
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetLatestFormVersions(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Data.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = new GetAllFormVersionsQuery();

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetLatestFormVersions())
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetLatestFormVersions(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}