﻿using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetAllFormVersionsQueryHandler : IRequestHandler<GetAllFormVersionsQuery, BaseMediatrResponse<GetAllFormVersionsQueryResponse>>
{
    private readonly IFormVersionRepository _formRepository;


    public GetAllFormVersionsQueryHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;

    }

    public async Task<BaseMediatrResponse<GetAllFormVersionsQueryResponse>> Handle(GetAllFormVersionsQuery request, CancellationToken cancellationToken)
    {
        var queryResponse = new BaseMediatrResponse<GetAllFormVersionsQueryResponse>();
        try
        {
            var data = await _formRepository.GetLatestFormVersions();

            queryResponse.Value.Data = [.. data];

            queryResponse.Success = true;
        }
        catch (Exception ex)
        {
            queryResponse.ErrorMessage = ex.Message;
        }

        return queryResponse;
    }
}