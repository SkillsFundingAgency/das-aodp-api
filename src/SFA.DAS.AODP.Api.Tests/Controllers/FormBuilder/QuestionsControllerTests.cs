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
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Question;
using SFA.DAS.AODP.Application.Commands.FormBuilder;

namespace SFA.DAS.AODP.Api.Tests.Controllers.FormBuilder.QuestionsControllerTests
{
    public class QuestionsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.Controllers.FormBuilder.QuestionsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.Controllers.FormBuilder.QuestionsController _controller;

        public QuestionsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.FormBuilder.QuestionsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.FormBuilder.QuestionsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetQuestionByIdQuery>();
            //var response = _fixture.Create<GetQuestionByIdQueryResponse>();
            var response = new GetQuestionByIdQueryResponse()
            {
                Id = Guid.NewGuid(),
                PageId = Guid.NewGuid(),
                Title = "",
                Key = Guid.NewGuid(),
                Hint = "",
                Order = 0,
                Required = false,
                Type = "",
                TextInput = new(),
                NumberInput = new(),
                Checkbox = new(),
                DateInput = new(),
                FileUpload = new(),
                Options = new(),
                Routes = new(),
                Editable = true
            };
            BaseMediatrResponse<GetQuestionByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetQuestionByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetByIdAsync(request.FormVersionId, request.PageId, request.SectionId, request.QuestionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetQuestionByIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetQuestionByIdQuery>(q =>
                        q.PageId == request.PageId
                        && q.FormVersionId == request.FormVersionId
                        && q.SectionId == request.SectionId
                        && q.QuestionId == request.QuestionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetQuestionByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<CreateQuestionCommand>();
            var response = _fixture.Create<CreateQuestionCommandResponse>();
            BaseMediatrResponse<CreateQuestionCommandResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateQuestionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.CreateAsync(request.FormVersionId, request.SectionId, request.PageId, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CreateQuestionCommandResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResult()
        {
            // Arrange
            //var request = _fixture.Create<UpdateQuestionCommand>();
            var request = new UpdateQuestionCommand()
            {
                Id = Guid.NewGuid(),
                FormVersionId = Guid.NewGuid(),
                SectionId = Guid.NewGuid(),

                PageId = Guid.NewGuid(),
                Title = "",
                Hint = "",
                Required = false
            };
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateQuestionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.UpdateAsync(request.FormVersionId, request.SectionId, request.PageId, request.Id, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task MoveUpAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<MoveQuestionUpCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MoveQuestionUpCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveUpAsync(request.FormVersionId, request.SectionId, request.PageId, request.QuestionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<MoveQuestionUpCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<MoveQuestionUpCommand>(q =>
                        q.PageId == request.PageId
                        && q.FormVersionId == request.FormVersionId
                        && q.SectionId == request.SectionId
                        && q.QuestionId == request.QuestionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task MoveDownAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<MoveQuestionDownCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MoveQuestionDownCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveDownAsync(request.FormVersionId, request.SectionId, request.PageId, request.QuestionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<MoveQuestionDownCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<MoveQuestionDownCommand>(q =>
                        q.PageId == request.PageId
                        && q.FormVersionId == request.FormVersionId
                        && q.SectionId == request.SectionId
                        && q.QuestionId == request.QuestionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task RemoveAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<DeleteQuestionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteQuestionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.DeleteByIdAsync(request.QuestionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteQuestionCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<DeleteQuestionCommand>(q =>
                        q.QuestionId == request.QuestionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }
    }
}