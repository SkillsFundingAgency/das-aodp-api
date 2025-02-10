using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class DeleteSectionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public readonly Guid SectionId;

    public DeleteSectionCommand(Guid sectionId)
    {
        SectionId = sectionId;
    }
}