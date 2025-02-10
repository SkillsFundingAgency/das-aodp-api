using MediatR;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class MoveFormUpCommandHandler : IRequestHandler<MoveFormUpCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IFormVersionRepository _formRepository;

    public MoveFormUpCommandHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(MoveFormUpCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();
        response.Success = false;

        try
        {
            var found = await _formRepository.MoveFormVersionOrderUp(request.FormVersionId);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
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
