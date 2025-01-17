using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

public class CreateFormCommand : IRequest<CreateFormCommandResponse>
{
    public FormVersion FormVersion { get; set; } 
}

