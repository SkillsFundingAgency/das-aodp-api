using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class CreateQualificationDiscussionHistoryNoteForFundingOffersCommandHandler : IRequestHandler<CreateQualificationDiscussionHistoryNoteForFundingOffersCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        private readonly IQualificationFundingsRepository _qualificationFundingsRepository;

        public CreateQualificationDiscussionHistoryNoteForFundingOffersCommandHandler(IQualificationDiscussionHistoryRepository repository, IQualificationFundingsRepository qualificationFundingsRepository)
        {
            _qualificationDiscussionHistoryRepository = repository;
            _qualificationFundingsRepository = qualificationFundingsRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(CreateQualificationDiscussionHistoryNoteForFundingOffersCommand request, CancellationToken cancellationToken)
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

                if (request.QualificationId == Guid.Empty)
                {
                    response.ErrorMessage = "Qualification Id is required";
                    response.Success = false;
                    return response;
                }

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
                        if (!string.IsNullOrWhiteSpace(qf.Comments)) qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                        qualificationDiscussionHistoryNotes.AppendLine();
                    }
                }
                else
                {
                    qualificationDiscussionHistoryNotes.AppendLine("No funding offers have been approved");
                }

                await _qualificationDiscussionHistoryRepository.CreateAsync(new QualificationDiscussionHistory
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