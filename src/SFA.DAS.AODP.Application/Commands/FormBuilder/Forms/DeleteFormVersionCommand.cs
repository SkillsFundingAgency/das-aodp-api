﻿using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class DeleteFormVersionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public readonly Guid FormVersionId;

    public DeleteFormVersionCommand(Guid formVersionId)
    {
        FormVersionId = formVersionId;
    }
}