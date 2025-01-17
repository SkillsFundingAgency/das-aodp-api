﻿using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryResponse : BaseResponse
{
    public List<Section> Data { get; set; }
}
