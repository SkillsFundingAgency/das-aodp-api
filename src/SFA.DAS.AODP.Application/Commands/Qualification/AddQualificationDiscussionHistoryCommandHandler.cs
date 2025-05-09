﻿using MediatR;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Commands.Qualification;

public class AddQualificationDiscussionHistoryCommandHandler : IRequestHandler<AddQualificationDiscussionHistoryCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IQualificationsRepository _qualificationsRepository;

    public AddQualificationDiscussionHistoryCommandHandler(IQualificationsRepository qualificationsRepository)
    {
        _qualificationsRepository = qualificationsRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(AddQualificationDiscussionHistoryCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            var qualificationDiscussionHistory = new QualificationDiscussionHistory()
            {
                UserDisplayName = request.UserDisplayName,
                Notes = request.Notes,
                Title = "Comment added"
            };
            await _qualificationsRepository.AddQualificationDiscussionHistory(qualificationDiscussionHistory, request.QualificationReference);
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
