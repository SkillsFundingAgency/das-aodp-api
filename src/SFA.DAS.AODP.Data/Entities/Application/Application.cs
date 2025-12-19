using SFA.DAS.AODP.Data.Entities.FormBuilder;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class Application
    {

        public Guid Id { get; set; }
        public Guid FormVersionId { get; set; }
        public string Name { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string Owner { get; set; }
        public bool? Submitted { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string? QualificationNumber { get; set; }
        public string? Status { get; set; }
        public bool? NewMessage { get; set; }

        public Guid OrganisationId { get; set; }
        public string? AwardingOrganisationName { get; set; }
        public string? AwardingOrganisationUkprn { get; set; }

        public virtual List<ApplicationPage> Pages { get; set; }
        public virtual ApplicationReview ApplicationReview { get; set; }
        public virtual FormVersion FormVersion { get; set; }

        public string? WithdrawnBy { get; set; }
        public DateTime? WithdrawnAt { get; set; }

    }
}
