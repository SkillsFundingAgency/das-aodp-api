using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.AODP.Api.Tests.Controllers.Application
{
    public class ApplicationsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.Controllers.Application.ApplicationsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.Controllers.Application.ApplicationsController _controller;

        public ApplicationsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.Application.ApplicationsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.Application.ApplicationsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormsQuery>();
            var response = _fixture.Create<GetApplicationFormsQueryResponse>();
            BaseMediatrResponse<GetApplicationFormsQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormsQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationFormsQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.IsAny<GetApplicationFormsQuery>(), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationFormsQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationMetadataByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationMetadataByIdQuery>();
            var response = _fixture.Create<GetApplicationMetadataByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationMetadataByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationMetadataByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationMetadataByIdAsync(request.ApplicationId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationMetadataByIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationMetadataByIdQuery>(q =>
                        q.ApplicationId == request.ApplicationId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationMetadataByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationPageByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationPageByIdQuery>();
            //var response = _fixture.Create<GetApplicationPageByIdQueryResponse>();
            var response = new GetApplicationPageByIdQueryResponse
            {
                Id = Guid.NewGuid(),
                Title = "",
                Description = "",
                Order = 0,
                TotalSectionPages = 0,
                Questions = new(),
            };
            BaseMediatrResponse<GetApplicationPageByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationPageByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationPageByIdAsync(request.PageOrder, request.SectionId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationPageByIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationPageByIdQuery>(q =>
                        q.PageOrder == request.PageOrder
                        && q.SectionId == request.SectionId
                        && q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationPageByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationFormByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormByIdQuery>();
            var response = _fixture.Create<GetApplicationFormByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetFormVersionByIdAsync(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationFormByIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationFormByIdQuery>(q =>
                        q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationFormByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationByOrganisationIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationsByOrganisationIdQuery>();
            var response = _fixture.Create<GetApplicationsByOrganisationIdQueryResponse>();
            BaseMediatrResponse<GetApplicationsByOrganisationIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationsByOrganisationIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationsByOrganisationId(request.OrganisationId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationsByOrganisationIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationsByOrganisationIdQuery>(q =>
                        q.OrganisationId == request.OrganisationId
            ), default), Times.Once()); var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationsByOrganisationIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationSectionByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormStatusByApplicationIdQuery>();
            var response = _fixture.Create<GetApplicationFormStatusByApplicationIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormStatusByApplicationIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationSectionsForFormByIdAsync(request.ApplicationId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationFormStatusByApplicationIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationFormStatusByApplicationIdQuery>(q =>
                        q.ApplicationId == request.ApplicationId
                        && q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationFormStatusByApplicationIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationSectionStatusByApplicationIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationSectionStatusByApplicationIdQuery>();
            var response = _fixture.Create<GetApplicationSectionStatusByApplicationIdQueryResponse>();
            BaseMediatrResponse<GetApplicationSectionStatusByApplicationIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationSectionStatusByApplicationIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationPagesForSectionByIdAsync(request.ApplicationId, request.SectionId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationSectionStatusByApplicationIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationSectionStatusByApplicationIdQuery>(q =>
                        q.ApplicationId == request.ApplicationId
                        && q.SectionId == request.SectionId
                        && q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationSectionStatusByApplicationIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationFormStatusByApplicationIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormStatusByApplicationIdQuery>();
            var response = _fixture.Create<GetApplicationFormStatusByApplicationIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormStatusByApplicationIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormStatusByApplicationIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationSectionsForFormByIdAsync(request.ApplicationId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationFormStatusByApplicationIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationFormStatusByApplicationIdQuery>(q =>
                        q.ApplicationId == request.ApplicationId
                        && q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationFormStatusByApplicationIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationPageAnswersByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationPageAnswersByPageIdQuery>();
            var response = new GetApplicationPageAnswersByPageIdQueryResponse
            {
                Questions = new()
            };

            //var response = _fixture.Create<GetApplicationPageAnswersByPageIdQueryResponse>
            //    (
            //    );
            BaseMediatrResponse<GetApplicationPageAnswersByPageIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationPageAnswersByPageIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationPageAnswersByIdAsync(request.ApplicationId, request.PageId, request.SectionId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationPageAnswersByPageIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationPageAnswersByPageIdQuery>(q =>
                        q.ApplicationId == request.ApplicationId
                        && q.PageId == request.PageId
                        && q.SectionId == request.SectionId
                        && q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationPageAnswersByPageIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<CreateApplicationCommand>();
            var response = _fixture.Create<CreateApplicationCommandResponse>();
            BaseMediatrResponse<CreateApplicationCommandResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateApplicationCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.CreateAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CreateApplicationCommandResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResult()
        {
            // Arrange
            //var request = _fixture.Create<UpdatePageAnswersCommand>();
            var request = new UpdatePageAnswersCommand()
            {
                ApplicationId = Guid.NewGuid(),
                PageId = Guid.NewGuid(),
                FormVersionId = Guid.NewGuid(),
                SectionId = Guid.NewGuid(),
                Questions = new(),
                Routing = new UpdatePageAnswersCommand.Route()
            };

            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdatePageAnswersCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.UpdateAnswersAsync(request.FormVersionId, request.SectionId, request.PageId, request.ApplicationId, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task RemoveAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<DeleteApplicationCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteApplicationCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.DeleteApplicationByIdAsync(request.ApplicationId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteApplicationCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<DeleteApplicationCommand>(q =>
                        q.ApplicationId == request.ApplicationId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetRelatedQualificationForApplicationAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetRelatedQualificationForApplicationQuery>();
            var response = _fixture.Create<GetRelatedQualificationForApplicationQueryResponse>();
            BaseMediatrResponse<GetRelatedQualificationForApplicationQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRelatedQualificationForApplicationQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetRelatedQualificationForApplicationAsync(request.ApplicationId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetRelatedQualificationForApplicationQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetRelatedQualificationForApplicationQuery>(q =>
                        q.ApplicationId == request.ApplicationId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetRelatedQualificationForApplicationQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

    }
}