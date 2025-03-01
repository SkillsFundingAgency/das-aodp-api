﻿using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

public class GetApplicationPageAnswersByPageIdQueryHandler : IRequestHandler<GetApplicationPageAnswersByPageIdQuery, BaseMediatrResponse<GetApplicationPageAnswersByPageIdQueryResponse>>
{
    private readonly IApplicationQuestionAnswerRepository _applicationQuestionAnswerRepository;

    public GetApplicationPageAnswersByPageIdQueryHandler(IApplicationQuestionAnswerRepository repository)
    {
        _applicationQuestionAnswerRepository = repository;
    }

    public async Task<BaseMediatrResponse<GetApplicationPageAnswersByPageIdQueryResponse>> Handle(GetApplicationPageAnswersByPageIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationPageAnswersByPageIdQueryResponse>();
        response.Success = false;
        try
        {
            List<ApplicationQuestionAnswer> result = await _applicationQuestionAnswerRepository.GetAnswersByApplicationAndPageId(request.ApplicationId, request.PageId);
            response.Value = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
