namespace SFA.DAS.AODP.Data.Repositories.FundingOffer
{
    public interface IFundingOfferRepository
    {
        Task<List<Data.Entities.Offer.FundingOffer>> GetFundingOffersAsync();
    }
}