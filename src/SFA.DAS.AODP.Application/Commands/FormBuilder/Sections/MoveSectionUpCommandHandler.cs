using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class MoveSectionUpCommandHandler(ISectionRepository sectionRepository) : IRequestHandler<MoveSectionUpCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly ISectionRepository _sectionRepository = sectionRepository;

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(MoveSectionUpCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>()
        {
        };

        try
        {
            if (!await _sectionRepository.IsSectionEditable(request.SectionId)) throw new RecordLockedException();

            var res = await _sectionRepository.MoveSectionOrderUp(request.SectionId);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
