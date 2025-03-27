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
            var discussuionHistory = new QualificationDiscussionHistory
            {
                UserDisplayName = request.UserDisplayName,
                Notes = request.Notes,
            }; 
            var status = await _qualificationsRepository.UpdateQualificationStatus(request.QualificationReference, request.ProcessStatusId, request.Version);
            discussuionHistory.Title = $"Updated status to: {status.Name}";
            await _qualificationsRepository.AddQualificationDiscussionHistory(discussuionHistory, request.QualificationReference);
            response.Success = true;
        }
        catch (RecordWithNameNotFoundException ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = new NotFoundWithNameException(request.QualificationReference);
        }
        catch (NoForeignKeyException ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = new DependantNotFoundException(request.ProcessStatusId);
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
