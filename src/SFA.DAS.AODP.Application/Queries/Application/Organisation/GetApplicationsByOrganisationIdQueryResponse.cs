﻿public class GetApplicationsByOrganisationIdQueryResponse
{
    public List<Application> Applications { get; set; } = new();

    public class Application
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public bool Submitted { get; set; }
        public string? Owner { get; set; }
        public string? Reference { get; set; }
        public Guid FormVersionId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Status { get; set; }
        public bool NewMessage { get; set; }
    }

    public static implicit operator GetApplicationsByOrganisationIdQueryResponse(List<SFA.DAS.AODP.Data.Entities.Application.Application> applications)
    {
        GetApplicationsByOrganisationIdQueryResponse response = new();

        foreach (var app in applications)
        {
            response.Applications.Add(new Application
            {
                Id = app.Id,
                Name = app.Name,
                Owner = app.Owner,
                Reference = app.ReferenceId.ToString(),
                CreatedDate = app.CreatedAt,
                Submitted = app.Submitted ?? false,
                SubmittedDate = app.SubmittedAt,
                FormVersionId = app.FormVersionId,
                UpdatedDate = app.UpdatedAt,
                Status = app.Status,
                NewMessage = app.NewMessage ?? false
            });

        }
        return response;

    }
}
