using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

public class GetApplicationFormByIdQueryResponse
{
    public string FormTitle { get; set; }

    public List<Section> Sections { get; set; } = new();

    public class Section
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
    }


    public static implicit operator GetApplicationFormByIdQueryResponse(FormVersion form)
    {
        GetApplicationFormByIdQueryResponse response = new()
        {
            FormTitle = form.Title,
            Sections = new()
        };

        foreach (var section in form.Sections)
        {
            response.Sections.Add(new()
            {
                Order = section.Order,
                Id = section.Id,
                Title = section.Title,
            });
        }

        return response;
    }
}