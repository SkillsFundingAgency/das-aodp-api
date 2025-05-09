﻿using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormVersionByIdQueryHandler : IRequestHandler<GetFormVersionByIdQuery, BaseMediatrResponse<GetFormVersionByIdQueryResponse>>
{
    private readonly IFormVersionRepository _formRepository;


    public GetFormVersionByIdQueryHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;

    }

    public async Task<BaseMediatrResponse<GetFormVersionByIdQueryResponse>> Handle(GetFormVersionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetFormVersionByIdQueryResponse>();

        try
        {
            var formVersion = await _formRepository.GetFormVersionByIdAsync(request.FormVersionId);
            response.Value = formVersion;
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.InnerException = new NotFoundException(ex.Id);
            response.Success = false;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
            response.Success = false;
        }

        return response;
    }
}
