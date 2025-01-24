﻿using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

/// <exception cref="NotFoundException"></exception>
public class DeleteFormVersionCommandResponse : BaseResponse
{
    public bool Data { get; set; }
}