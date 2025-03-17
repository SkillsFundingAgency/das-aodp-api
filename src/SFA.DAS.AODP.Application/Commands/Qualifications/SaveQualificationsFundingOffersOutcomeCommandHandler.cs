using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersOutcomeCommandHandler : IRequestHandler<SaveQualificationsFundingOffersOutcomeCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingFeedbackRepository _repository;

        public SaveQualificationsFundingOffersOutcomeCommandHandler(IQualificationFundingFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersOutcomeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _repository.GetQualificationFundingFeedbackDetailsByIdAsync(request.QualificationVersionId);
                if (review == null)
                {
                    review = new QualificationFundingFeedbacks
                    {
                        QualificationVersionId = request.QualificationVersionId,
                    };
                    await _repository.CreateAsync(review);
                    response.Success = true; 
                }
                else
                {
                    review.Comments = request.Comments;
                    review.Status = request.Approved ? ApplicationStatus.Approved.ToString() : ApplicationStatus.NotApproved.ToString();
                    await _repository.UpdateAsync(review);
                    response.Success = true;
                }
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