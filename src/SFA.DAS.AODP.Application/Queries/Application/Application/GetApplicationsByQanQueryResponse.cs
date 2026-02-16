using System;
using System.Collections.Generic;
using System.Linq;

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
        public string? Status { get; set; }
        public int ReferenceId { get; set; }
    }

    public static GetApplicationsByQanQueryResponse FromEntities(IEnumerable<SFA.DAS.AODP.Data.Entities.Application.Application>? applications)
    {
        if (applications is null)
        {
            return new GetApplicationsByQanQueryResponse();
        }

        return new GetApplicationsByQanQueryResponse
        {
            Applications = applications.Select(app => new Application
            {
                Id = app.Id,
                Name = app.Name,
                CreatedDate = app.CreatedAt,
                SubmittedDate = app.SubmittedAt,
                Status = app.Status,
                ReferenceId = app.ReferenceId
            }).ToList()
        };
    }
}
