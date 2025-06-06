﻿namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetAllFormVersionsQueryResponse
{
    public List<FormVersion> Data { get; set; }

    public class FormVersion
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string Title { get; set; }
        public DateTime Version { get; set; }
        public string Status { get; set; }

        public string Description { get; set; }
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }

        public static implicit operator FormVersion(Data.Entities.FormBuilder.FormVersion entity)
        {
            return (new()
            {
                Id = entity.Id,
                FormId = entity.FormId,
                Title = entity.Title,
                Version = entity.Version,
                Status = entity.Status,
                Description = entity.Description,
                Order = entity.Form.Order,
                DateCreated = entity.DateCreated,
            });
        }

    }
}

  