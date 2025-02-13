using SFA.DAS.AODP.Models.Archived.Forms.Validators;

namespace SFA.DAS.AODP.Models.Archived.Forms.FormBuilder.Question;

public class Question
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Required { get; set; }
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public List<Option> MultiChoice { get; set; } = new List<Option>();
    public List<RoutingPoint> RoutingPoints { get; set; } = new List<RoutingPoint>();
    public TextValidator? TextValidator { get; set; }
    public IntegerValidator? IntegerValidator { get; set; }
    public DecimalValidator? DecimalValidator { get; set; }
    public DateValidator? DateValidator { get; set; }
    public MultiChoiceValidator? MultiChoiceValidator { get; set; }
    public BooleanValidaor? BooleanValidaor { get; set; }
}
