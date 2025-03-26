using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Routes;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForFormQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRouteRepository> _repositoryMock;
        private readonly GetRoutingInformationForFormQueryHandler _handler;
        public GetRoutingInformationForFormQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IRouteRepository>>();
            _handler = _fixture.Create<GetRoutingInformationForFormQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_RoutingInformationForForm_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var query = new GetRoutingInformationForFormQuery();
            query.FormVersionId = formVersionId;
            var response = new List<View_QuestionRoutingDetail>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetQuestionRoutingDetailsByFormVersionId(formVersionId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionRoutingDetailsByFormVersionId(formVersionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Sections.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NotFoundException_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            RecordNotFoundException ex = new RecordNotFoundException(formVersionId);

            var query = new GetRoutingInformationForFormQuery();
            query.FormVersionId = formVersionId;
            var response = new List<View_QuestionRoutingDetail>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetQuestionRoutingDetailsByFormVersionId(formVersionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionRoutingDetailsByFormVersionId(formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.IsType<NotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Exception ex = new Exception();

            var query = new GetRoutingInformationForFormQuery();
            query.FormVersionId = formVersionId;
            var response = new List<View_QuestionRoutingDetail>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetQuestionRoutingDetailsByFormVersionId(formVersionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionRoutingDetailsByFormVersionId(formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}