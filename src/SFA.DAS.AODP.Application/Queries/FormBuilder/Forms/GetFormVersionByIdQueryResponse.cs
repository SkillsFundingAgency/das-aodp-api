﻿using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;


/// <exception cref="NotFoundException"></exception>
public class GetFormVersionByIdQueryResponse
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Title { get; set; }
    public DateTime Version { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string DescriptionHTML { get; set; }
    public int Order { get; set; }
    public List<Section> Sections { get; set; }

    public static implicit operator GetFormVersionByIdQueryResponse(FormVersion formVersion)
    {
        return new GetFormVersionByIdQueryResponse()
        {
            Id = formVersion.Id,
            FormId = formVersion.FormId,
            Description = formVersion.Description,
            DescriptionHTML = formVersion.DescriptionHTML,
            Order = formVersion.Form.Order,
            Title = formVersion.Title,
            Version = formVersion.Version,
            Status = formVersion.Status,
            Sections = [.. formVersion.Sections]

        };
    }


    public class Section
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }

        public static implicit operator Section(Data.Entities.FormBuilder.Section section)
        {
            return new()
            {
                Id = section.Id,
                Key = section.Key,
                Order = section.Order,
                Title = section.Title
            };
        }
    }



}