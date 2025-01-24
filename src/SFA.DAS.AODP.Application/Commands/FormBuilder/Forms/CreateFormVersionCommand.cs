﻿using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommand : IRequest<CreateFormVersionCommandResponse>
{
    public readonly FormVersion Data;

    public CreateFormVersionCommand(FormVersion data)
    {
        Data = data;
    }

    public class FormVersion
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Version { get; set; }
        public FormStatus Status { get; set; }
        public bool Archived { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }
    }
}