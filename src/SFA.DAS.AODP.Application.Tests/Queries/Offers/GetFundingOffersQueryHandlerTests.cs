using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;
using SFA.DAS.AODP.Application.Queries.Offers;
using SFA.DAS.AODP.Data.Entities.Offer;

namespace SFA.DAS.AODP.Application.Tests.Queries.Offers
{
    public class GetFundingOffersQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFundingOfferRepository> _repositoryMock;
        private readonly GetFundingOffersQueryHandler _handler;
        public GetFundingOffersQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IFundingOfferRepository>>();
            _handler = _fixture.Create<GetFundingOffersQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_FundingOffers_Are_Returned()
        {
            // Arrange
            var query = new GetFundingOffersQuery();
            var response = new List<FundingOffer>()
            {
                new FundingOffer()
                {
                    Id = Guid.NewGuid(),
                    Name = " "
                }
            };

            _repositoryMock.Setup(x => x.GetFundingOffersAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFundingOffersAsync(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Offers.Count);
            Assert.Single(result.Value.Offers);
            Assert.Equal(response.First().Id, result.Value.Offers.First().Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = new GetFundingOffersQuery();

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetFundingOffersAsync())
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFundingOffersAsync(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}