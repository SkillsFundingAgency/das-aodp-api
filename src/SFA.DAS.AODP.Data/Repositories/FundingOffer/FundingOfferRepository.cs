using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Repositories.FundingOffer
{
    public class FundingOfferRepository : IFundingOfferRepository
    {
        private readonly IApplicationDbContext _context;

        public FundingOfferRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Data.Entities.Offer.FundingOffer>> GetFundingOffersAsync() => await _context.FundingOffers.ToListAsync();

    }
}
