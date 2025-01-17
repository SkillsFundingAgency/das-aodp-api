using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

public class UpdateFormCommand : IRequest<UpdateFormCommandResponse>
{
    public FormVersion FormVersion { get; set; }
}
