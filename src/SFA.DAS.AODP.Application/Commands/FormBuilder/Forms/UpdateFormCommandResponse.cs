namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

using SFA.DAS.AODP.Models.Forms.FormBuilder;

public class UpdateFormCommandResponse : BaseResponse 
{ 
    public FormVersion FormVersion { get; set; }
}