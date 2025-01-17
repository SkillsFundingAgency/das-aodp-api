﻿using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

public class UpdateSectionCommand : IRequest<UpdateSectionCommandResponse>
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? NextSectionId { get; set; }
}
