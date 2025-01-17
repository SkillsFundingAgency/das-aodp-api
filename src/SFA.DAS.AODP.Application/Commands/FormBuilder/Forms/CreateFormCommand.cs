using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormCommand : IRequest<CreateFormCommandResponse>
{
    public string Name { get; set; }
    public DateTime Version { get; set; }
    public bool Published { get; set; }
    public string Key { get; set; }
    public string ApplicationTrackingTemplate { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
}

