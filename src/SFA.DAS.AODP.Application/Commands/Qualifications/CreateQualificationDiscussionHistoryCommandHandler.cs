using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class CreateQualificationDiscussionHistoryCommandHandler : IRequestHandler<CreateQualificationDiscussionHistoryCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationDiscussionHistoryRepository _repository;
        private readonly IQualificationFundingsRepository _qualificationFundingsRepository;
        private readonly IQualificationRepository _qualificationRepository;

        public CreateQualificationDiscussionHistoryCommandHandler(IQualificationDiscussionHistoryRepository repository, IQualificationFundingsRepository qualificationFundingsRepository, IQualificationRepository qualificationRepository)
        {
            _repository = repository;
            _qualificationFundingsRepository = qualificationFundingsRepository;
            _qualificationRepository = qualificationRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(CreateQualificationDiscussionHistoryCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var qualificationFundings = await _qualificationFundingsRepository.GetByIdAsync(request.QualificationVersionId);
                var qualification = await _qualificationRepository.GetByIdAsync(request.QualificationReference);
                if (qualification == null)
                {
                    response.ErrorMessage = "Qualification not found";
                    response.Success = false;
                    return response;
                }
                request.QualificationId = qualification.Id;

                string fundedOfferName = "";
                if (qualificationFundings != null && qualificationFundings.Count != 0)
                {
                    foreach (var qf in qualificationFundings)
                    {
                        fundedOfferName += qf.FundingOffer.Name + ", StartDate:" + qf.StartDate?.ToString("dd-MM-yyyy") + ", EndDate:" + qf.EndDate?.ToString("dd-MM-yyyy");
                    }
                }
                await _repository.CreateAsync(new QualificationDiscussionHistory
                {
                    QualificationId = request.QualificationId,
                    UserDisplayName = request.UserDisplayName,
                    Notes = "Qualification funding offers review - " + fundedOfferName,
                    ActionTypeId = request.ActionTypeId,
                    Timestamp = DateTime.Now
                });
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