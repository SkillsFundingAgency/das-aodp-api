namespace SFA.DAS.AODP.Data.Entities.FormBuilder.Validators;

public class DateValidator
{
    public bool Required { get; set; }
    public DateTime? GreaterThan { get; set; }
    public DateTime? LessThan { get; set; }
    public DateTime? EqualTo { get; set; }
    public DateTime? NotEqualTo { get; set; }

    public DateSpan? GreaterThanTimeInFuture { get; set; }
    public DateSpan? LessThanTimeInFuture { get; set; }
    public DateSpan? GreaterThanTimeInPast { get; set; }
    public DateSpan? LessThanTimeInPast { get; set; }
}

public record DateSpan(int Years, int Months, int Days);