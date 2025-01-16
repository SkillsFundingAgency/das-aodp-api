namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Form;

public class GetAllFormsQueryResponse : BaseResponse
{
    public List<Models.Forms.FormBuilder.Form> Data { get; set; }
}
