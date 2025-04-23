using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;
using static SFA.DAS.AODP.Application.Queries.FormBuilder.Forms.GetAllFormVersionsQueryResponse;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.FormBuilder.Questions
{
    public class GetQuestionByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQuestionRepository> _repositoryMock;
        private readonly Mock<IRouteRepository> _repositoryRouteMock;
        private readonly Mock<IFormVersionRepository> _repositoryFormMock;
        private readonly GetQuestionByIdQueryHandler _handler;
        public GetQuestionByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IQuestionRepository>>();
            _repositoryRouteMock = _fixture.Freeze<Mock<IRouteRepository>>();
            _repositoryFormMock = _fixture.Freeze<Mock<IFormVersionRepository>>();
            _handler = _fixture.Create<GetQuestionByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Question_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            var query = new GetQuestionByIdQuery();
            query.QuestionId = questionId;
            query.FormVersionId = formVersionId;
            query.SectionId = sectionId;
            query.PageId = pageId;

            var response = new Question()
                {
                    Id = questionId,
                    PageId = pageId,
                    Title = " ",
                    Type = " ",
                    Required = false,
                    Order = 1,
                    Key = Guid.NewGuid()
               };

            var responseRoutes = new List<View_QuestionRoutingDetail>()
            {
                new View_QuestionRoutingDetail()
                {
                    FormVersionId = formVersionId,
                    SectionId = sectionId,
                    PageId = pageId,
                    QuestionId = questionId,
                    OptionId = Guid.NewGuid(),
                    QuestionTitle = " ",
                    PageTitle = " ",
                    SectionTitle = " ",
                    OptionValue = " ",
                    QuestionOrder = 1,
                    PageOrder = 1,
                    SectionOrder = 1,
                    OptionOrder = 1,
                    EndForm = false,
                    EndSection = false
                }
            };
            bool isFormVersionEditable = true;

            _repositoryMock.Setup(x => x.GetQuestionByIdAsync(questionId))
                .ReturnsAsync(response);

            _repositoryRouteMock.Setup(x => x.GetQuestionRoutingDetailsByQuestionId(questionId))
                .ReturnsAsync(responseRoutes);

            _repositoryFormMock.Setup(x => x.IsFormVersionEditable(formVersionId))
               .ReturnsAsync(isFormVersionEditable);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionByIdAsync(questionId), Times.Once);
            _repositoryRouteMock.Verify(x => x.GetQuestionRoutingDetailsByQuestionId(questionId), Times.Once);
            _repositoryFormMock.Verify(x => x.IsFormVersionEditable(formVersionId), Times.Once);
            Assert.True(result.Success);
            Assert.Single(result.Value.Routes);
            Assert.Equal(response.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NotFoundException_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            var query = new GetQuestionByIdQuery();
            query.QuestionId = questionId;
            query.FormVersionId = formVersionId;
            query.SectionId = sectionId;
            query.PageId = pageId;

            RecordNotFoundException ex = new RecordNotFoundException(questionId);

            _repositoryMock.Setup(x => x.GetQuestionByIdAsync(questionId))
                .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionByIdAsync(questionId), Times.Once);
            Assert.False(result.Success);
            Assert.IsType<NotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_In_RepositoryMock_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            var query = new GetQuestionByIdQuery();
            query.QuestionId = questionId;
            query.FormVersionId = formVersionId;
            query.SectionId = sectionId;
            query.PageId = pageId;

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetQuestionByIdAsync(questionId))
                .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetQuestionByIdAsync(questionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_In_RepositoryRouteMock_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            var query = new GetQuestionByIdQuery();
            query.QuestionId = questionId;
            query.FormVersionId = formVersionId;
            query.SectionId = sectionId;
            query.PageId = pageId;

            Exception ex = new Exception();

            _repositoryRouteMock.Setup(x => x.GetQuestionRoutingDetailsByQuestionId(questionId))
                .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryRouteMock.Verify(x => x.GetQuestionRoutingDetailsByQuestionId(questionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_In_RepositoryFormMock_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            var query = new GetQuestionByIdQuery();
            query.QuestionId = questionId;
            query.FormVersionId = formVersionId;
            query.SectionId = sectionId;
            query.PageId = pageId;

            Exception ex = new Exception();

            _repositoryFormMock.Setup(x => x.IsFormVersionEditable(formVersionId))
               .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryFormMock.Verify(x => x.IsFormVersionEditable(formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}