using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersDetailsCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid QualificationVersionId { get; set; }
        public List<OfferFundingDetails> Details { get; set; } = new();

        public class OfferFundingDetails
        {
            public Guid FundingOfferId { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public string? Comments { get; set; }
        }
    }

}