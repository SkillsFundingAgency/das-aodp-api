﻿using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.FormBuilder.Pages
{
    public class GetPageByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IPageRepository> _repositoryMock;
        private readonly Mock<IRouteRepository> _repositoryRouteMock;
        private readonly Mock<IFormVersionRepository> _repositoryFormMock;
        private readonly GetPageByIdQueryHandler _handler;
        public GetPageByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IPageRepository>>();
            _repositoryRouteMock = _fixture.Freeze<Mock<IRouteRepository>>();
            _repositoryFormMock = _fixture.Freeze<Mock<IFormVersionRepository>>();
            _handler = _fixture.Create<GetPageByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Page_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            var query = new GetPageByIdQuery(pageId, sectionId, formVersionId);
            var response = new Page()
            {
                Id = pageId,
                SectionId = sectionId,
                Title = " ",
                Key = Guid.NewGuid(),
                Order = 1
            };

            _repositoryMock.Setup(x => x.GetPageByIdAsync(pageId))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPageByIdAsync(pageId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_NotFoundException_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            var query = new GetPageByIdQuery(pageId, sectionId, formVersionId);

            RecordNotFoundException ex = new RecordNotFoundException(sectionId);

            _repositoryMock.Setup(x => x.GetPageByIdAsync(pageId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPageByIdAsync(pageId), Times.Once);
            Assert.False(result.Success);
            Assert.IsType<NotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            var query = new GetPageByIdQuery(pageId, sectionId, formVersionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetPageByIdAsync(pageId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPageByIdAsync(pageId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}