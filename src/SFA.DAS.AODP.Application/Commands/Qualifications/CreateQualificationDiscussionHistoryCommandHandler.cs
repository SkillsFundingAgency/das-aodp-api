using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class CreateQualificationDiscussionHistoryCommandHandler : IRequestHandler<CreateQualificationDiscussionHistoryCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationDiscussionHistoryRepository _repository;
        private readonly IQualificationFundingsRepository _qualificationFundingsRepository;
        private readonly IQualificationsRepository _qualificationsRepository;

        public CreateQualificationDiscussionHistoryCommandHandler(IQualificationDiscussionHistoryRepository repository, IQualificationFundingsRepository qualificationFundingsRepository, IQualificationsRepository qualificationsRepository)
        {
            _repository = repository;
            _qualificationFundingsRepository = qualificationFundingsRepository;
            _qualificationsRepository = qualificationsRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(CreateQualificationDiscussionHistoryCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                if (string.IsNullOrEmpty(request.QualificationReference))
                {
                    response.ErrorMessage = "Qualification reference is required";
                    response.Success = false;
                    return response;
                }
                var qualification = await _qualificationsRepository.GetByIdAsync(request.QualificationReference) ?? throw new RecordWithNameNotFoundException("Qualification not found"); ;
                
                request.QualificationId = qualification.Id;

                var qualificationFundings = await _qualificationFundingsRepository.GetByIdAsync(request.QualificationVersionId);

                StringBuilder qualificationDiscussionHistoryNotes = new();
                qualificationDiscussionHistoryNotes.AppendLine("Feedback from DfE:");
                if (qualificationFundings != null && qualificationFundings.Count != 0)
                {
                    qualificationDiscussionHistoryNotes.AppendLine("The following offers have been approved:");
                    qualificationDiscussionHistoryNotes.AppendLine();

                    foreach (var qf in qualificationFundings)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine($"Offer: {qf.FundingOffer.Name}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                        qualificationDiscussionHistoryNotes.AppendLine();
                    }
                }
                else
                {
                    qualificationDiscussionHistoryNotes.AppendLine("No funding offers have been approved");
                }

                await _repository.CreateAsync(new QualificationDiscussionHistory
                {
                    QualificationId = request.QualificationId,
                    UserDisplayName = request.UserDisplayName,
                    Notes = qualificationDiscussionHistoryNotes.ToString(),
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