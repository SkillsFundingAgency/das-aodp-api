using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    [ExcludeFromCodeCoverage]
    public class SaveQualificationsFundingOffersOutcomeCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid QualificationVersionId { get; set; }
        public string? Comments { get; set; }
        public bool? Approved { get; set; }
        public Guid QualificationId { get; set; }
        public Guid ActionTypeId { get; set; }
        [AllowedCharacters(TextCharacterProfile.PersonName)]
        public string? UserDisplayName { get; set; }
        public bool? UpdateDiscussionHistory { get; set; } = true;

    }
}