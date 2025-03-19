using SFA.DAS.AODP.Data.Entities.Application;

public class GetApplicationFormStatusByApplicationIdQueryResponse
{
    public string ApplicationName { get; set; }
    public string? Reference { get; set; }
    public string? QualificationNumber { get; set; }

    public bool ReadyForSubmit { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string Owner { get; set; }
    public bool Submitted { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }

    public List<Section> Sections { get; set; } = new();

    public class Section
    {
        public Guid SectionId { get; set; }
        public int PagesRemaining { get; set; }
        public int SkippedPages { get; set; }
        public int TotalPages { get; set; }

    }

    public static implicit operator GetApplicationFormStatusByApplicationIdQueryResponse(Application application)
    {
        GetApplicationFormStatusByApplicationIdQueryResponse response = new()
        {
            ApplicationName = application.Name,
            Reference = application.ReferenceId.ToString(),
            Owner = application.Owner,
            Submitted = application.Submitted ?? false,
            SubmittedDate = application.SubmittedAt,
            QualificationNumber = application.QualificationNumber,
            UpdatedDate = application.UpdatedAt,
            Status = application.Status
        };

        return response;
    }
}