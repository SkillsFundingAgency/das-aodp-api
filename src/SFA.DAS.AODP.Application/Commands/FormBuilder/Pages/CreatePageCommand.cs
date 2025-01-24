﻿using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

public class CreatePageCommand : IRequest<CreatePageCommandResponse>
{
    public readonly Page Data;

    public CreatePageCommand(Page data)
    {
        Data = data;
    }

    public class Page
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid Key { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public int? NextPageId { get; set; }
    }
}