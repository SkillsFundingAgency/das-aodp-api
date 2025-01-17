using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class UpdatePageCommandRequest : IRequest<UpdatePageCommandResponse>
{
    public Page Data { get; set; }
}
