using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    [ExcludeFromCodeCoverage]
    public class SaveQfauFundingReviewDecisionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid ApplicationReviewId { get; set; }
        [AllowedCharacters(TextCharacterProfile.PersonName)]
        public string SentByName { get; set; }
        [AllowedCharacters(TextCharacterProfile.FreeText)]
        public string SentByEmail { get; set; }
    }
}