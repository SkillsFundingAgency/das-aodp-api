using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommandRequest : IRequest<CreateFormVersionCommandResponse>
{
    public FormVersion Data { get; set; }
}

