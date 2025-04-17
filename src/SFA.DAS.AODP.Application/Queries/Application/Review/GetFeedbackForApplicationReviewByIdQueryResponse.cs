namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetFeedbackForApplicationReviewByIdQueryResponse
    {
        public string? Owner { get; set; }
        public string Status { get; set; }
        public bool NewMessage { get; set; }
        public string UserType { get; set; }
        public string? Comments { get; set; }
        public Guid ApplicationId { get; set; }

        public List<Funding> FundedOffers { get; set; } = new();

        public class Funding
        {
            public Guid Id { get; set; }
            public Guid FundingOfferId { get; set; }
            public string FundedOfferName { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public string? Comments { get; set; }
        }


        public static implicit operator GetFeedbackForApplicationReviewByIdQueryResponse(Data.Entities.Application.ApplicationReviewFeedback feedback)
        {
            GetFeedbackForApplicationReviewByIdQueryResponse model = new()
            {
                Comments = feedback.Comments,
                NewMessage = feedback.NewMessage,
                Owner = feedback.Owner,
                Status = feedback.Status,
                UserType = feedback.Type,
                ApplicationId = feedback.ApplicationReview.ApplicationId
            };

            foreach (var funding in feedback.ApplicationReview?.ApplicationReviewFundings ?? [])
            {
                model.FundedOffers.Add(new()
                {
                    Id = funding.Id,
                    FundedOfferName = funding.FundingOffer.Name,
                    FundingOfferId = funding.FundingOffer.Id,
                    Comments = funding.Comments,
                    EndDate = funding.EndDate,
                    StartDate = funding.StartDate,
                });
            }

            return model;
        }
    }

}