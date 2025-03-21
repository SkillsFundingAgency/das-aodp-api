﻿using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class CreateSectionCommand : IRequest<BaseMediatrResponse<CreateSectionCommandResponse>>
{

    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid Key { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? NextSectionId { get; set; }
}
