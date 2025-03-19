using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewOutcomeCommandHandler : IRequestHandler<SaveQfauFundingReviewOutcomeCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewFeedbackRepository _repository;

        public SaveQfauFundingReviewOutcomeCommandHandler(IApplicationReviewFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQfauFundingReviewOutcomeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _repository.GetApplicationReviewFeedbackDetailsByReviewIdAsync(request.ApplicationReviewId, Models.Application.UserType.Qfau);

                review.Comments = request.Comments;
                review.Status = request.Approved ? ApplicationStatus.Approved.ToString() : ApplicationStatus.NotApproved.ToString();

                await _repository.UpdateAsync(review);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
                response.Success = false;
            }
            return response;
        }
    }

}