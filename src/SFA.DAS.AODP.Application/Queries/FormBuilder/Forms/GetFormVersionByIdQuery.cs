﻿using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormVersionByIdQuery : IRequest<BaseMediatrResponse<GetFormVersionByIdQueryResponse>>
{
    public readonly Guid FormVersionId;

    public GetFormVersionByIdQuery(Guid formVersionId)
    {
        FormVersionId = formVersionId;
    }
}