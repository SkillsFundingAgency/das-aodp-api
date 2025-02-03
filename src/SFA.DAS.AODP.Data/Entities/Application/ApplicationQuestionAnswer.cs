namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class ApplicationQuestionAnswer
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ApplicationPageId { get; set; }
        public string? TextValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public string? OptionsValue { get; set; }
        public decimal? NumberValue { get; set; }

        public virtual ApplicationPage ApplicationPage { get; set; }

    }
}
