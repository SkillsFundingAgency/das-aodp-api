using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Models.Rollover;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersDetailsCommandHandler : IRequestHandler<SaveQualificationsFundingOffersDetailsCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingsRepository _qualificationFundingsrepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        private readonly IFundingChangeCoordinator _fundingChangeCoordinator;

        public SaveQualificationsFundingOffersDetailsCommandHandler(
            IQualificationFundingsRepository repository,
            IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository,
            IFundingChangeCoordinator fundingChangeCoordinator)
        {
            _qualificationFundingsrepository = repository;
            _qualificationDiscussionHistoryRepository = qualificationDiscussionHistoryRepository;
            _fundingChangeCoordinator = fundingChangeCoordinator;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersDetailsCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fundedOffers = await _qualificationFundingsrepository.GetByIdAsync(request.QualificationVersionId);

                var changedDetails = request.Details
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

                if (changedDetails.Count > 0)
                {
                    var changeSet = FundingChangeSet.Create(changedDetails.Select(x =>
                        new FundingChangeKey(
                            RolloverSourceTypes.Ofqual,
                            request.QualificationVersionId,
                            x.Detail.FundingOfferId)));

                    await _fundingChangeCoordinator.ExecuteAsync(
                        changeSet,
                        async _ =>
                        {
                            foreach (var changedDetail in changedDetails)
                            {
                                changedDetail.Funding.StartDate = changedDetail.Detail.StartDate;
                                changedDetail.Funding.EndDate = changedDetail.Detail.EndDate;
                                changedDetail.Funding.Comments = changedDetail.Detail.Comments;
                            }

                            await _qualificationFundingsrepository.UpdateAsync(fundedOffers);
                            return true;
                        },
                        cancellationToken);
                }

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
                            qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate.ToFundingEndDateFormat()}");
                            qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate.ToFundingEndDateFormat()}");
                            if (!string.IsNullOrWhiteSpace(qf.Comments)) qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
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
