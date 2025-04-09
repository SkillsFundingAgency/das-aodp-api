using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersDetailsCommandHandler : IRequestHandler<SaveQualificationsFundingOffersDetailsCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingsRepository _qualificationFundingsrepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        public SaveQualificationsFundingOffersDetailsCommandHandler(IQualificationFundingsRepository repository, IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository)
        {
            _qualificationFundingsrepository = repository;
            _qualificationDiscussionHistoryRepository = qualificationDiscussionHistoryRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersDetailsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fundedOffers = await _qualificationFundingsrepository.GetByIdAsync(request.QualificationVersionId);

                foreach (var detail in request.Details)
                {
                    var offer = fundedOffers.FirstOrDefault(a => a.FundingOfferId == detail.FundingOfferId) ?? throw new RecordNotFoundException(detail.FundingOfferId);

                    offer.StartDate = detail.StartDate;
                    offer.EndDate = detail.EndDate;
                    offer.Comments = detail.Comments;
                }

                await _qualificationFundingsrepository.UpdateAsync(fundedOffers);

                if (request.UpdateDiscussionHistory == true)
                {
                    StringBuilder qualificationDiscussionHistoryNotes = new();
                    qualificationDiscussionHistoryNotes.AppendLine("Feedback from DfE:");
                    if (request.Details != null && request.Details.Count != 0)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine("The following offers details have been selected:");
                        qualificationDiscussionHistoryNotes.AppendLine();

                        foreach (var qf in request.Details)
                        {
                            qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate?.ToString("dd-MM-yyyy")}");
                            qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate?.ToString("dd-MM-yyyy")}");
                            qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                            qualificationDiscussionHistoryNotes.AppendLine();
                        }
                    }
                    else
                    {
                        qualificationDiscussionHistoryNotes.AppendLine("No funding offers have been selected");
                    }

                    await _qualificationDiscussionHistoryRepository.CreateAsync(new QualificationDiscussionHistory
                    {
                        QualificationId = request.QualificationId,
                        UserDisplayName = request.UserDisplayName,
                        Notes = qualificationDiscussionHistoryNotes.ToString(),
                        ActionTypeId = request.ActionTypeId,
                        Timestamp = DateTime.Now
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