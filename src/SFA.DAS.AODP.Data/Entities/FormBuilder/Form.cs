using SFA.DAS.AODP.Models.Form;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities;

public class Form
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public virtual List<FormVersion> Versions { get; set; }


}
