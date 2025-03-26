using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Forms
{
    public class GetFormVersionByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFormVersionRepository> _repositoryMock;
        private readonly GetFormVersionByIdQueryHandler _handler;
        public GetFormVersionByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IFormVersionRepository>>();
            _handler = _fixture.Create<GetFormVersionByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_FormVersion_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var query = new GetFormVersionByIdQuery(formVersionId);
            var response = new FormVersion()
            {
                Id = formVersionId,
                Form = new()
                {
                    Order = 1
                },
                Sections = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid()
                    }
                }
            };

            _repositoryMock.Setup(x => x.GetFormVersionByIdAsync(formVersionId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFormVersionByIdAsync(formVersionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NotFoundException_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var query = new GetFormVersionByIdQuery(formVersionId);

            RecordNotFoundException ex = new RecordNotFoundException(formVersionId);

            _repositoryMock.Setup(x => x.GetFormVersionByIdAsync(formVersionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFormVersionByIdAsync(formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.IsType<NotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var query = new GetFormVersionByIdQuery(formVersionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetFormVersionByIdAsync(formVersionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFormVersionByIdAsync(formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}