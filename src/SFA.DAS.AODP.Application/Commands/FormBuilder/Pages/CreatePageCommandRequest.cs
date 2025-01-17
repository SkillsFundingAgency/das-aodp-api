using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class CreatePageCommandRequest : IRequest<CreatePageCommandResponse>
{
    public Page Data { get; set; }
}
