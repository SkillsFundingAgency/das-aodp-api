using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    [ExcludeFromCodeCoverage]
    public class SaveQualificationsFundingOffersDetailsCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid QualificationVersionId { get; set; }
        public List<OfferFundingDetails> Details { get; set; } = new();
        public Guid QualificationId { get; set; }
        public Guid ActionTypeId { get; set; }
        [AllowedCharacters(TextCharacterProfile.PersonName)]
        public string? UserDisplayName { get; set; }
        public bool? UpdateDiscussionHistory { get; set; } = true;

        public class OfferFundingDetails
        {
            public Guid FundingOfferId { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            [AllowedCharacters(TextCharacterProfile.FreeText)]
            public string? Comments { get; set; }
        }
    }

}