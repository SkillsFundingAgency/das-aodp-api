﻿using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record PublishFormVersionCommand(Guid FormVersionId) : IRequest<PublishFormVersionCommandResponse>;
