using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    [ExcludeFromCodeCoverage]
    public class SaveQfauFundingReviewOffersCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid ApplicationReviewId { get; set; }
        public List<Guid> SelectedOfferIds { get; set; } = new();
    }

}