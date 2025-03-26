using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Sections
{
    public class GetSectionByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ISectionRepository> _repositoryMock;
        private readonly Mock<IFormVersionRepository> _repositoryFormMock;
        private readonly GetSectionByIdQueryHandler _handler;
        public GetSectionByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<ISectionRepository>>();
            _repositoryFormMock = _fixture.Freeze<Mock<IFormVersionRepository>>();
            _handler = _fixture.Create<GetSectionByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Section_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            var query = new GetSectionByIdQuery(sectionId, formVersionId);
            var response = new Section()
            {
                Id = sectionId
            };

            bool hasRoutesForSection = true;

            bool isFormVersionEditable = true;

            _repositoryMock.Setup(x => x.GetSectionByIdAsync(sectionId))
                .ReturnsAsync(response);

            //_repositoryMock.Setup(x => x.HasRoutesForSectionAsync(sectionId))
            //    .ReturnsAsync(hasRoutesForSection);

            //_repositoryFormMock.Setup(x => x.IsFormVersionEditable(formVersionId))
            //    .ReturnsAsync(hasRoutesForSection);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionByIdAsync(sectionId), Times.Once);
            //_repositoryMock.Verify(x => x.HasRoutesForSectionAsync(formVersionId), Times.Once);
            //_repositoryFormMock.Verify(x => x.IsFormVersionEditable(formVersionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NotFoundException_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();


            var query = new GetSectionByIdQuery(sectionId, formVersionId);

            RecordNotFoundException ex = new RecordNotFoundException(sectionId);

            _repositoryMock.Setup(x => x.GetSectionByIdAsync(sectionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionByIdAsync(sectionId), Times.Once);
            Assert.False(result.Success);
            Assert.IsType<NotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            var query = new GetSectionByIdQuery(sectionId, formVersionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetSectionByIdAsync(sectionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionByIdAsync(sectionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}