
namespace SFA.DAS.AODP.Data.Entities;

public class TextValidator
{
    public bool Required { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
}
