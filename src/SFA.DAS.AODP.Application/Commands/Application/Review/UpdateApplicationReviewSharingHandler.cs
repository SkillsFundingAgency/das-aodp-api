using MediatR;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class UpdateApplicationReviewSharingHandler : IRequestHandler<UpdateApplicationReviewSharingCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationReviewRepository _applicationReviewRepository;
        private readonly IApplicationReviewFeedbackRepository _applicationReviewFeedbackRepository;

        public UpdateApplicationReviewSharingHandler(IApplicationReviewRepository applicationReviewRepository, IApplicationReviewFeedbackRepository applicationReviewFeedbackRepository)
        {
            _applicationReviewRepository = applicationReviewRepository;
            _applicationReviewFeedbackRepository = applicationReviewFeedbackRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateApplicationReviewSharingCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _applicationReviewRepository.GetByIdAsync(request.ApplicationReviewId);

                if (request.ApplicationReviewUserType == UserType.Ofqual)
                {
                    review.SharedWithOfqual = request.ShareApplication;
                }
                else if (request.ApplicationReviewUserType == UserType.SkillsEngland)
                {
                    review.SharedWithSkillsEngland = request.ShareApplication;
                }

                if (!review.ApplicationReviewFeedbacks.Exists(f => f.Type == request.ApplicationReviewUserType.ToString()))
                {
                    await _applicationReviewFeedbackRepository.CreateAsync(new ApplicationReviewFeedback()
                    {
                        ApplicationReviewId = review.Id,
                        Type = request.ApplicationReviewUserType.ToString(),
                        NewMessage = true,
                        Status = ApplicationStatus.InReview.ToString()
                    });
                    //TODO add message to timeline for relevant user type
                }

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