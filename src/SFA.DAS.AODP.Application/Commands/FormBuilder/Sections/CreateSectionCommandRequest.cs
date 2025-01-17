using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class CreateSectionCommandRequest : IRequest<CreateSectionCommandResponse>
{
    public Section Data {  get; set; }
}
