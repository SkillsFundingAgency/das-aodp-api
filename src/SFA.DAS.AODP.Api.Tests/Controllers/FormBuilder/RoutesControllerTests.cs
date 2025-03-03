using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Routes;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Routes;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

namespace SFA.DAS.AODP.Api.Tests.Controllers.FormBuilder.RoutesControllerTests
{
    public class RoutesControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.Controllers.FormBuilder.RoutesController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.Controllers.FormBuilder.RoutesController _controller;

        public RoutesControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.FormBuilder.RoutesController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.FormBuilder.RoutesController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAvailableSectionsAndPagesForRouting_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetAvailableSectionsAndPagesForRoutingQuery>();
            var response = _fixture.Create<GetAvailableSectionsAndPagesForRoutingQueryResponse>();
            BaseMediatrResponse<GetAvailableSectionsAndPagesForRoutingQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAvailableSectionsAndPagesForRoutingQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetAvailableSectionsAndPagesForRouting(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAvailableSectionsAndPagesForRoutingQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetAvailableSectionsAndPagesForRoutingQuery>(q =>
                        q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetAvailableSectionsAndPagesForRoutingQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetRoutesByFormVersionId_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetRoutingInformationForFormQuery>();
            //var response = _fixture.Create<GetRoutingInformationForFormQueryResponse>();
            var response = new GetRoutingInformationForFormQueryResponse()
            {
                Sections = new(),
                Editable = true
            };
            BaseMediatrResponse<GetRoutingInformationForFormQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRoutingInformationForFormQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetRoutesByFormVersionId(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetRoutingInformationForFormQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetRoutingInformationForFormQuery>(q =>
                        q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetRoutingInformationForFormQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetAvailableQuestionsForRouting_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetAvailableQuestionsForRoutingQuery>();
            var response = _fixture.Create<GetAvailableQuestionsForRoutingQueryResponse>();
            BaseMediatrResponse<GetAvailableQuestionsForRoutingQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAvailableQuestionsForRoutingQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetAvailableQuestionsForRouting(request.PageId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAvailableQuestionsForRoutingQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetAvailableQuestionsForRoutingQuery>(q =>
                        q.PageId == request.PageId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetAvailableQuestionsForRoutingQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetQuestionRoutingInformation_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetRoutingInformationForQuestionQuery>();
            var response = _fixture.Create<GetRoutingInformationForQuestionQueryResponse>();
            BaseMediatrResponse<GetRoutingInformationForQuestionQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRoutingInformationForQuestionQuery>(), default))
                .Returns(Task.FromResult(wrapper));

            // Act
            var result = await _controller.GetQuestionRoutingInformation(request.QuestionId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetRoutingInformationForQuestionQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetRoutingInformationForQuestionQuery>(q =>
                        q.QuestionId == request.QuestionId
                        && q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetRoutingInformationForQuestionQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task ConfigureRoutingForQuestion_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<ConfigureRoutingForQuestionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ConfigureRoutingForQuestionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.ConfigureRoutingForQuestion(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }
    }
}