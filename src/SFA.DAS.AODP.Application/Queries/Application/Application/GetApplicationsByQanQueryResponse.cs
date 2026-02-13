namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationsByQanQueryResponse
{


    public List<Application> Applications { get; set; } = new();

    public class Application
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
    }

    public static implicit operator GetApplicationsByQanQueryResponse(List<SFA.DAS.AODP.Data.Entities.Application.Application> applications)
    {
        GetApplicationsByQanQueryResponse response = new();

        foreach (var app in applications)
        {
            response.Applications.Add(new Application
            {
                Id = app.Id,
                Name = app.Name,
                CreatedDate = app.CreatedAt,
                SubmittedDate = app.SubmittedAt,
            });

        }
        return response;

    }
}
