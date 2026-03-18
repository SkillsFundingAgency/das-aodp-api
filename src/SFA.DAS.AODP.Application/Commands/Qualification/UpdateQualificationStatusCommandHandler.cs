using MediatR;
using SFA.DAS.AODP.Data.Exceptions;
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
            var discussionHistory = new QualificationDiscussionHistory
            {
                UserDisplayName = request.UserDisplayName,
                Notes = request.Notes,
            }; 
            
            var qualificationVersion = await _qualificationsRepository.GetQualificationVersionByQanAsync(request.QualificationReference, cancellationToken);

            if (qualificationVersion is null)
            {
                throw new RecordWithNameNotFoundException(request.QualificationReference);
            }

            // If the current qualification version process status is NOT as the requested process status, then update the qualification version process status and add a discussion history record.
            if (qualificationVersion.ProcessStatus.Id != request.ProcessStatusId)
            {
                qualificationVersion.SetLifecycleStatus(ProcessStatusLookup.FromId(request.ProcessStatusId));

                discussionHistory.Title = $"Updated status to: {ProcessStatusLookup.FromId(request.ProcessStatusId).Name}";
                await _qualificationsRepository.AddQualificationDiscussionHistory(discussionHistory, request.QualificationReference);
                response.Success = true;
            }

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
