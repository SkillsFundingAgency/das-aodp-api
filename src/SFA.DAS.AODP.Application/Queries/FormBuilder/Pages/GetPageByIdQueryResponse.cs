﻿using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetPageByIdQueryResponse : BaseResponse
{
    public Page Data { get; set; }

    public class Page
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string Title { get; set; }
        public Guid Key { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public int? NextPageId { get; set; }
    }
}