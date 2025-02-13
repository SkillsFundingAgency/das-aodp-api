namespace SFA.DAS.AODP.Models.Form
{
    public enum FormVersionStatus
    {
        Draft,
        Published,
        Archived
    }

    public enum FormStatus
    {
        Active, // Available to AOs if published
        Inactive, // Not available to AOs even if published
        Deleted // Hidden from both AOs and admins
    }
}
