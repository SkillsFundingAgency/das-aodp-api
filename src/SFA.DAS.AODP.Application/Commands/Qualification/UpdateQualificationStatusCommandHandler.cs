using MediatR;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Commands.Qualification;

public class UpdateQualificationStatusCommandHandler : IRequestHandler<UpdateQualificationStatusCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IQualificationsRepository _qualificationsRepository;

    public UpdateQualificationStatusCommandHandler(IQualificationsRepository qualificationsRepository)
    {
        _qualificationsRepository = qualificationsRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateQualificationStatusCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            await _qualificationsRepository.UpdateQualificationStatus(request.QualificationReference, request.Status);
            response.Success = true;
        }
        catch (RecordWithNameNotFoundException ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = new NotFoundWithNameException(request.QualificationReference);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }

        return response;
    }
}
