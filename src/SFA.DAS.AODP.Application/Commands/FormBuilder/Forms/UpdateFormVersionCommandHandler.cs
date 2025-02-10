using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommandHandler : IRequestHandler<UpdateFormVersionCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IFormVersionRepository _formRepository;


    public UpdateFormVersionCommandHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;

    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateFormVersionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            if (!await _formRepository.IsFormVersionEditable(request.FormVersionId)) throw new RecordLockedException();

            var formVersion = await _formRepository.GetFormVersionByIdAsync(request.FormVersionId);
            formVersion.Title = request.Name;
            formVersion.Description = request.Description;


            await _formRepository.Update(formVersion);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.InnerException = new NotFoundException(ex.Id);
            response.Success = false;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
