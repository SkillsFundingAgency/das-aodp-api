using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Routes;
using Microsoft.EntityFrameworkCore.Metadata;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForQuestionQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQuestionRepository> _repositoryMock;
        private readonly Mock<IPageRepository> _repositoryPageMock;
        private readonly Mock<ISectionRepository> _repositorySectionMock;
        private readonly GetRoutingInformationForQuestionQueryHandler _handler;
        public GetRoutingInformationForQuestionQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IQuestionRepository>>();
            _repositoryPageMock = _fixture.Freeze<Mock<IPageRepository>>();
            _repositorySectionMock = _fixture.Freeze<Mock<ISectionRepository>>();
            _handler = _fixture.Create<GetRoutingInformationForQuestionQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_RoutingInformationForQuestion_Is_Returned()
        {
            // Arrange
            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetRoutingInformationForQuestionQuery();
            query.FormVersionId = formVersionId;
            query.QuestionId = questionId;

            var response = new Question()
            {
                Id = questionId,
                Title = " ",
                Page = new()
                {
                    Id = pageId,
                    Title = " ",
                    Section = new()
                    {
                        Id = sectionId,
                        Title = " "
                    }
                }
            };

            _repositoryMock.Setup(x => x.GetQuestionDetailForRoutingAsync(questionId))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionDetailForRoutingAsync(questionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Id, result.Value.QuestionId);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NotFoundException_Is_Handled()
        {
            // Arrange
            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetRoutingInformationForQuestionQuery();
            query.FormVersionId = formVersionId;
            query.QuestionId = questionId;

            RecordNotFoundException ex = new RecordNotFoundException(questionId);

            _repositoryMock.Setup(x => x.GetQuestionDetailForRoutingAsync(questionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionDetailForRoutingAsync(questionId), Times.Once);
            Assert.False(result.Success);
            Assert.IsType<NotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetRoutingInformationForQuestionQuery();
            query.FormVersionId = formVersionId;
            query.QuestionId = questionId;

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetQuestionDetailForRoutingAsync(questionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionDetailForRoutingAsync(questionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}