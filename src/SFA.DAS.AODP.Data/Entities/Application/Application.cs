namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class Application
    {
        public Guid Id { get; set; }
        public Guid FormVersionId { get; set; }
        public Guid? OrganisationId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public virtual List<ApplicationPage> Pages { get; set; }
    }
}
