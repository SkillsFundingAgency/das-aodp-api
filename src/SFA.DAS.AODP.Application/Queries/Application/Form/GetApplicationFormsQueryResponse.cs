using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

public class GetApplicationFormsQueryResponse
{
    public List<Form> Forms { get; set; }

    public class Form
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }

    public static implicit operator GetApplicationFormsQueryResponse(List<FormVersion> forms)
    {
        GetApplicationFormsQueryResponse response = new()
        {
            Forms = new()
        };

        foreach (FormVersion form in forms)
        {
            response.Forms.Add(new()
            {
                Description = form.Description,
                Order = form.Form.Order,
                Id = form.Id,
                Title = form.Title,
            });
        }

        return response;
    }
}