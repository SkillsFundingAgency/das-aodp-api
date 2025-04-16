using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersOutcomeCommandHandler : IRequestHandler<SaveQualificationsFundingOffersOutcomeCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingFeedbackRepository _qualificationFundingFeedbackRepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;

        public SaveQualificationsFundingOffersOutcomeCommandHandler(IQualificationFundingFeedbackRepository repository, IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository)
        {
            _qualificationFundingFeedbackRepository = repository;
            _qualificationDiscussionHistoryRepository = qualificationDiscussionHistoryRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersOutcomeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var qualificationFundingFeedback = await _qualificationFundingFeedbackRepository.GetByIdAsync(request.QualificationVersionId);
                if (qualificationFundingFeedback == null)
                {
                    qualificationFundingFeedback = new QualificationFundingFeedbacks
                    {
                        QualificationVersionId = request.QualificationVersionId,
                        Comments = request.Comments,
                        Approved = request?.Approved

                    };
                    await _qualificationFundingFeedbackRepository.CreateAsync(qualificationFundingFeedback);
                }
                else
                {
                    qualificationFundingFeedback.Comments = request.Comments;
                    qualificationFundingFeedback.Approved = request?.Approved;
                    await _qualificationFundingFeedbackRepository.UpdateAsync(qualificationFundingFeedback);
                }

                if (request.UpdateDiscussionHistory == true)
                {
                    StringBuilder qualificationDiscussionHistoryNotes = new();
                    qualificationDiscussionHistoryNotes.AppendLine("Feedback from DfE:");
                    if (request != null && request.Approved.HasValue)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine("The funding qualification");
                        qualificationDiscussionHistoryNotes.AppendLine($"overall outcome selected is: {(request.Approved == true ? "Approved" : "Rejected")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Comments: {request.Comments}");
                        qualificationDiscussionHistoryNotes.AppendLine();
                    }
                    else
                    {
                        qualificationDiscussionHistoryNotes.AppendLine("The overall outcome for funding this qualification not been selected");
                    }

                    await _qualificationDiscussionHistoryRepository.CreateAsync(new QualificationDiscussionHistory
                    {
                        QualificationId = request.QualificationId,
                        UserDisplayName = request.UserDisplayName,
                        Notes = qualificationDiscussionHistoryNotes.ToString(),
                        ActionTypeId = request.ActionTypeId,
                        Timestamp = DateTime.UtcNow
                    });
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