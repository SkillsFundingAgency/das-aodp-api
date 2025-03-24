using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetFeedbackForApplicationReviewByIdQueryHandler : IRequestHandler<GetFeedbackForApplicationReviewByIdQuery, BaseMediatrResponse<GetFeedbackForApplicationReviewByIdQueryResponse>>
    {
        private readonly IApplicationReviewFeedbackRepository _applicationRepository;

        public GetFeedbackForApplicationReviewByIdQueryHandler(IApplicationReviewFeedbackRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<BaseMediatrResponse<GetFeedbackForApplicationReviewByIdQueryResponse>> Handle(GetFeedbackForApplicationReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetFeedbackForApplicationReviewByIdQueryResponse>();
            response.Success = false;
            try
            {
                if (!Enum.TryParse(request.UserType, out UserType userType)) throw new Exception("Invalid user type provided");

                var result = await _applicationRepository.GeyByReviewIdAndUserType(request.ApplicationReviewId, userType);
                response.Value = result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.InnerException = ex;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }

}