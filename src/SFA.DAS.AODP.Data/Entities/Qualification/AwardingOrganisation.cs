namespace SFA.DAS.AODP.Data.Entities.Qualification;

public partial class AwardingOrganisation
{
    public Guid Id { get; set; }
    
    public int? Ukprn { get; set; }

    public string? RecognitionNumber { get; set; }

    public string? NameLegal { get; set; }

    public string? NameOfqual { get; set; }

    public string? NameGovUk { get; set; }

    public string? Name_Dsi { get; set; }

    public string? Acronym { get; set; }

    public virtual ICollection<FundedQualification> Qualifications { get; set; } = new List<FundedQualification>();

    public virtual ICollection<QualificationVersions> QualificationVersions { get; set; } = new List<QualificationVersions>();
}
