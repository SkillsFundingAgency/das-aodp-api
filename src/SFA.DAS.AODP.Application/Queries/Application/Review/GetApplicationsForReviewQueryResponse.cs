using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationsForReviewQueryResponse
    {
        public List<Application> Applications { get; set; } = new();
        public int TotalRecordsCount { get; set; }
        public class Application
        {
            public Guid Id { get; set; }
            public Guid ApplicationReviewId { get; set; }
            public string Name { get; set; }
            public DateTime LastUpdated { get; set; }
            public int Reference { get; set; }
            public string? Qan { get; set; }
            public string? AwardingOrganisation { get; set; }

            public string? Owner { get; set; }
            public string Status { get; set; }
            public bool NewMessage { get; set; }
        }

        public static GetApplicationsForReviewQueryResponse Map(List<ApplicationReviewFeedback> reviews, int totalRecords)
        {
            GetApplicationsForReviewQueryResponse response = new()
            {
                TotalRecordsCount = totalRecords
            };

            foreach (var review in reviews)
            {
                var application = new Application
                {
                    Id = review.ApplicationReview.Application.Id,
                    Name = review.ApplicationReview.Application.Name,
                    LastUpdated = review.ApplicationReview.Application.UpdatedAt,
                    Reference = review.ApplicationReview.Application.ReferenceId,
                    Qan = review.ApplicationReview.Application.QualificationNumber,
                    Status = review.Status,
                    NewMessage = review.NewMessage,
                    Owner = review.Owner,
                    AwardingOrganisation = review.ApplicationReview.Application.AwardingOrganisationName,
                    ApplicationReviewId = review.ApplicationReviewId
                };



                response.Applications.Add(application);
            }


            return response;
        }
    }
}