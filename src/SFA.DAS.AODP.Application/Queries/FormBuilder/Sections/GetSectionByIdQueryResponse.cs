﻿using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryResponse(Section data) : BaseResponse
{
<<<<<<< HEAD
    public Section Data { get; set; } = data;
=======
    public Section Data { get; set; }

    public class Section
    {
        public Guid Id { get; set; }
        public Guid FormVersionId { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? NextSectionId { get; set; }
    }
>>>>>>> ab4b648a65944bfdc978e773d93dae3cd911f872
}