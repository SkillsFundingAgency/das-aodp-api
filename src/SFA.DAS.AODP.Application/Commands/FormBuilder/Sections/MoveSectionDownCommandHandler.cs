using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class MoveSectionDownCommandHandler(ISectionRepository sectionRepository) : IRequestHandler<MoveSectionDownCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly ISectionRepository _sectionRepository = sectionRepository;

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(MoveSectionDownCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>()
        {
        };

        try
        {
            if (!await _sectionRepository.IsSectionEditable(request.SectionId)) throw new RecordLockedException();

            var res = await _sectionRepository.MoveSectionOrderDown(request.SectionId);
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
