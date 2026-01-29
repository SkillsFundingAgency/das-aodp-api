using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationsForReviewQueryHandler : IRequestHandler<GetApplicationsForReviewQuery, BaseMediatrResponse<GetApplicationsForReviewQueryResponse>>
    {
        private readonly IApplicationReviewFeedbackRepository _applicationReviewRepository;

        public GetApplicationsForReviewQueryHandler(IApplicationReviewFeedbackRepository applicationReviewRepository) => _applicationReviewRepository = applicationReviewRepository;

        public async Task<BaseMediatrResponse<GetApplicationsForReviewQueryResponse>> Handle(GetApplicationsForReviewQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetApplicationsForReviewQueryResponse>();
            response.Success = false;
            try
            {
                if (!Enum.TryParse(request.ReviewUser, out UserType userType)) throw new Exception("Invalid user type provided");

                var criteria = new ApplicationReviewSearchCriteria
                {
                    ReviewType = userType,
                    Offset = request.Offset ?? 0,
                    Limit = request.Limit ?? 10,
                    IncludeApplicationWithNewMessages = request.ApplicationsWithNewMessages,
                    ApplicationStatuses = request.ApplicationStatuses,
                    ApplicationSearch = request.ApplicationSearch,
                    AwardingOrganisationSearch = request.AwardingOrganisationSearch,
                    ReviewerSearch = request.ReviewerSearch,
                    UnassignedOnly = request.UnassignedOnly
                };

                var applicationReviewsWithCount = await _applicationReviewRepository.GetApplicationReviews(
                    criteria
                );

                response.Value = GetApplicationsForReviewQueryResponse.Map(applicationReviewsWithCount.Item1, applicationReviewsWithCount.Item2);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}