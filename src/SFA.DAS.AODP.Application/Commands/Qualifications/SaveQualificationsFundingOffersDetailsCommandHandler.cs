using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Infrastructure.Extensions;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersDetailsCommandHandler : IRequestHandler<SaveQualificationsFundingOffersDetailsCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingsRepository _qualificationFundingsrepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        public SaveQualificationsFundingOffersDetailsCommandHandler(
            IQualificationFundingsRepository repository,
            IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository)
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

                await ApplyFundingChangesAsync(fundedOffers, request.Details);

                if (request.UpdateDiscussionHistory == true)
                {
                    await _qualificationDiscussionHistoryRepository.CreateAsync(new QualificationDiscussionHistory
                    {
                        QualificationId = request.QualificationId,
                        UserDisplayName = request.UserDisplayName,
                        Notes = BuildDiscussionHistoryNotes(request.Details),
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

        private async Task ApplyFundingChangesAsync(
            List<QualificationFundings> fundedOffers,
            List<SaveQualificationsFundingOffersDetailsCommand.OfferFundingDetails> details)
        {
            var changedDetails = details
                .Select(detail => new
                {
                    Detail = detail,
                    Funding = fundedOffers.FirstOrDefault(a => a.FundingOfferId == detail.FundingOfferId)
                        ?? throw new RecordNotFoundException(detail.FundingOfferId)
                })
                .Where(x =>
                    x.Funding.StartDate != x.Detail.StartDate ||
                    x.Funding.EndDate != x.Detail.EndDate ||
                    x.Funding.Comments != x.Detail.Comments)
                .ToList();

            if (changedDetails.Count == 0)
            {
                return;
            }

            foreach (var changedDetail in changedDetails)
            {
                changedDetail.Funding.UpdateFunding(
                    changedDetail.Detail.StartDate,
                    changedDetail.Detail.EndDate,
                    changedDetail.Detail.Comments);
            }

            await _qualificationFundingsrepository.UpdateAsync(fundedOffers);
        }

        private static string BuildDiscussionHistoryNotes(List<SaveQualificationsFundingOffersDetailsCommand.OfferFundingDetails> details)
        {
            var notes = new StringBuilder();
            notes.AppendLine("Feedback from DfE:");

            if (details == null || details.Count == 0)
            {
                notes.AppendLine("No funding offers have been selected");
                return notes.ToString();
            }

            notes.AppendLine("The following offers details have been selected:");
            notes.AppendLine();

            foreach (var qf in details)
            {
                notes.AppendLine($"Start date: {qf.StartDate.ToFundingEndDateFormat()}");
                notes.AppendLine($"End date: {qf.EndDate.ToFundingEndDateFormat()}");
                if (!string.IsNullOrWhiteSpace(qf.Comments))
                {
                    notes.AppendLine($"Comments: {qf.Comments}");
                }
                notes.AppendLine();
            }

            return notes.ToString();
        }
    }

}
