﻿namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

using SFA.DAS.AODP.Models.Forms.FormBuilder;

public class GetAllFormsQueryResponse : BaseResponse
{
    public List<Form> Data { get; set; }
}