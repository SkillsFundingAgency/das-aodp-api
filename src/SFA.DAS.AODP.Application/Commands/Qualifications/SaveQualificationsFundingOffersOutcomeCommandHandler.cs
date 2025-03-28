﻿using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

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
                var qualificationFundingFeedback = await _repository.GetByIdAsync(request.QualificationVersionId);
                if (qualificationFundingFeedback == null)
                {
                    qualificationFundingFeedback = new QualificationFundingFeedbacks
                    {
                        QualificationVersionId = request.QualificationVersionId,
                        Comments = request.Comments,
                        Approved = request?.Approved

                    };
                    await _repository.CreateAsync(qualificationFundingFeedback);
                    response.Success = true; 
                }
                else
                {
                    qualificationFundingFeedback.Comments = request.Comments;
                    qualificationFundingFeedback.Approved = request?.Approved;
                    await _repository.UpdateAsync(qualificationFundingFeedback);
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