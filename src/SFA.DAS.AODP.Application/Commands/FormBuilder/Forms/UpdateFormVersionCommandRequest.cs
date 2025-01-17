using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommandRequest : IRequest<UpdateFormVersionCommandResponse>
{
    public FormVersion Data { get; set; }
}
