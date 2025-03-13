using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQualificationsFundingOffersCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid QualificationVersionId { get; set; }
        public List<Guid> SelectedOfferIds { get; set; } = new();
    }

}