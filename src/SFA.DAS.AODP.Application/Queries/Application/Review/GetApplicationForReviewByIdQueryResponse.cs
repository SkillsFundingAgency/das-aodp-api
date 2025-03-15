namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationForReviewByIdQueryResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationReviewId { get; set; }

        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }
        public int Reference { get; set; }
        public string? Qan { get; set; }
        public string? AwardingOrganisation { get; set; }

        public bool SharedWithSkillsEngland { get; set; }
        public bool SharedWithOfqual { get; set; }

        public List<Funding> FundedOffers { get; set; } = new();
        public List<Feedback> Feedbacks { get; set; } = new();

        public class Feedback
        {
            public string? Owner { get; set; }
            public string Status { get; set; }
            public bool NewMessage { get; set; }
            public string UserType { get; set; }
            public string? Comments { get; set; }
        }

        public class Funding
        {
            public Guid Id { get; set; }
            public Guid FundingOfferId { get; set; }
            public string FundedOfferName { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public string? Comments { get; set; }
        }

        public static implicit operator GetApplicationForReviewByIdQueryResponse(Data.Entities.Application.ApplicationReview review)
        {
            GetApplicationForReviewByIdQueryResponse model = new()
            {
                Id = review.Application.Id,
                AwardingOrganisation = review.Application.AwardingOrganisationName,
                LastUpdated = review.Application.UpdatedAt,
                Name = review.Application.Name,
                Qan = review.Application.QualificationNumber,
                Reference = review.Application.ReferenceId,
                SharedWithOfqual = review.SharedWithOfqual,
                SharedWithSkillsEngland = review.SharedWithSkillsEngland,
                ApplicationReviewId = review.Id
            };

            foreach (var feedback in review.ApplicationReviewFeedbacks ?? [])
            {
                model.Feedbacks.Add(new()
                {
                    Comments = feedback.Comments,
                    NewMessage = feedback.NewMessage,
                    Owner = feedback.Owner,
                    Status = feedback.Status,
                    UserType = feedback.Type
                });
            }

            foreach (var funding in review.ApplicationReviewFundings ?? [])
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