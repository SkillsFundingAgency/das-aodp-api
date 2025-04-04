using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class CreateQualificationDiscussionHistoryCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid Id { get; set; }

        public Guid QualificationVersionId { get; set; }

        public Guid QualificationId { get; set; }

        public string? QualificationReference { get; set; }

        public Guid ActionTypeId { get; set; }

        public string? UserDisplayName { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}