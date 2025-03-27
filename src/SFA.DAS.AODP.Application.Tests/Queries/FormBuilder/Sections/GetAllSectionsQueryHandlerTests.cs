using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Sections
{
    public class GetAllSectionsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ISectionRepository> _repositoryMock;
        private readonly GetAllSectionsQueryHandler _handler;
        public GetAllSectionsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<ISectionRepository>>();
            _handler = _fixture.Create<GetAllSectionsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Sections_Are_Returned()
        {
            // Arrange
            Guid formId = Guid.NewGuid();

            var query = new GetAllSectionsQuery(formId);
            var response = new List<Section>()
            {
                new Section()
                {
                    Id = Guid.NewGuid(),
                    FormVersionId = Guid.NewGuid(),
                    Key = Guid.NewGuid(),
                    Order = 1,
                    Title = " "
                }
            };

            _repositoryMock.Setup(x => x.GetSectionsForFormAsync(formId))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionsForFormAsync(formId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Data.Count);
            Assert.Single(result.Value.Data);
            Assert.Equal(response.First().Id, result.Value.Data.First().Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formId = Guid.NewGuid();

            var query = new GetAllSectionsQuery(formId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetSectionsForFormAsync(formId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionsForFormAsync(formId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}