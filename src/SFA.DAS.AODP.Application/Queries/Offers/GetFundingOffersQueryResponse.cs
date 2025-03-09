namespace SFA.DAS.AODP.Application.Queries.Offers
{
    public class GetFundingOffersQueryResponse
    {
        public class FundingOffer
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public List<FundingOffer> Offers { get; set; } = new();

        public static implicit operator GetFundingOffersQueryResponse(List<Data.Entities.Offer.FundingOffer> offers) 
        {
            GetFundingOffersQueryResponse model = new();

            foreach (var offer in offers)
            {
                model.Offers.Add(new()
                {
                    Id = offer.Id,
                    Name = offer.Name,
                });
            }

            return model;
        }
    }
}
