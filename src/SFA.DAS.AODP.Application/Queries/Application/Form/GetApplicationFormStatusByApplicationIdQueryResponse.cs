using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

public class GetApplicationFormStatusByApplicationIdQueryResponse : BaseResponse
{
    public string ApplicationName { get; set; }
    public string Reference { get; set; }

    public bool ReadyForSubmit { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string Owner { get; set; }
    public bool Submitted { get; set; }

    public List<Section> Sections { get; set; } = new();

    public class Section
    {
        public Guid SectionId { get; set; }
        public int PagesRemaining { get; set; }
    }

    public static implicit operator GetApplicationFormStatusByApplicationIdQueryResponse(Application application)
    {
        GetApplicationFormStatusByApplicationIdQueryResponse response = new()
        {
            ApplicationName = application.Name,
            Reference = application.Reference,
            Owner = application.Owner,
            Submitted = application.Submitted ?? false,
            SubmittedDate = application.SubmittedAt
        };

        return response;
    }
}