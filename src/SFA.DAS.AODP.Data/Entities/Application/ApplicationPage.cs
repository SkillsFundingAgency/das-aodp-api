using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class ApplicationPage
    {
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public Guid ApplicationId { get; set; }
        public string Status { get; set; }
        public Guid? SkippedByQuestionId { get; set; }

        public virtual List<ApplicationQuestionAnswer> QuestionAnswers { get; set; }
        public virtual Application Application { get; set; }
        public virtual Page Page { get; set; }
    }

    public enum ApplicationPageStatus
    {
        NotStarted, Completed, Skipped
    }
}
