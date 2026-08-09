using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Infrastructure.Extensions;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersCommandHandler : IRequestHandler<SaveQualificationsFundingOffersCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingsRepository _qualificationFundingsrepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        private readonly IFundingOfferRepository _fundingOfferRepository;
        private readonly IQualificationsRepository _qualificationsRepository;
        public SaveQualificationsFundingOffersCommandHandler(
            IQualificationFundingsRepository repository,
            IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository,
            IFundingOfferRepository fundingOfferRepository,
            IQualificationsRepository qualificationsRepository)
        {
            _qualificationFundingsrepository = repository;
            _qualificationDiscussionHistoryRepository = qualificationDiscussionHistoryRepository;
            _fundingOfferRepository = fundingOfferRepository;
            _qualificationsRepository = qualificationsRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var qualificationfundedOffers = await _qualificationFundingsrepository.GetByIdAsync(request.QualificationVersionId);
                var fundingOffers = await _fundingOfferRepository.GetFundingOffersAsync();
                var operationalStartDate = await _qualificationsRepository.GetQualificationVersionOperationalStartDateById(request.QualificationVersionId, cancellationToken);

                List<QualificationFundings> create = new();

                foreach (var offerId in request.SelectedOfferIds)
                {
                    var offer = qualificationfundedOffers.FirstOrDefault(a => a.FundingOfferId == offerId);
                    if (offer == null)
                    {
                        create.Add(QualificationFundings.Create(
                            request.QualificationVersionId,
                            offerId,
                            operationalStartDate,
                            null));
                    }
                }

                if (create.Count > 0)
                {
                    await _qualificationFundingsrepository.CreateAsync(create);
                }

                if (request.UpdateDiscussionHistory == true)
                {
                    StringBuilder qualificationDiscussionHistoryNotes = new();
                    qualificationDiscussionHistoryNotes.AppendLine("Feedback from DfE:");
                    if (create.Count > 0)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine("The following offers have been selected:");
                        qualificationDiscussionHistoryNotes.AppendLine();

                        foreach (var qf in create)
                        {
                            var fundingOffer = fundingOffers.FirstOrDefault(a => a.Id == qf.FundingOfferId);
                            if (fundingOffer != null)
                            {
                                qualificationDiscussionHistoryNotes.AppendLine($"Offer: {fundingOffer.Name}");
                                qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate.ToFundingEndDateFormat()}");
                                qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate.ToFundingEndDateFormat()}");
                                if (!string.IsNullOrWhiteSpace(qf.Comments)) qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                                qualificationDiscussionHistoryNotes.AppendLine();
                            }
                        }
                    }

                if (create.Count == 0)
                {
                    qualificationDiscussionHistoryNotes.AppendLine("Funding has been approved but no offers have been selected");
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
