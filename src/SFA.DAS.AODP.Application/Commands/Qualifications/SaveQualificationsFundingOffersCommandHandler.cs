using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersCommandHandler : IRequestHandler<SaveQualificationsFundingOffersCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IQualificationFundingsRepository _qualificationFundingsrepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        private readonly IFundingOfferRepository _fundingOfferRepository;



        public SaveQualificationsFundingOffersCommandHandler(IQualificationFundingsRepository repository, IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository, IFundingOfferRepository fundingOfferRepository)
        {
            _qualificationFundingsrepository = repository;
            _qualificationDiscussionHistoryRepository = qualificationDiscussionHistoryRepository;
            _fundingOfferRepository = fundingOfferRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQualificationsFundingOffersCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var qualificationfundedOffers = await _qualificationFundingsrepository.GetByIdAsync(request.QualificationVersionId);
                var fundingOffers = await _fundingOfferRepository.GetFundingOffersAsync();

                List<QualificationFundings> create = new();
                List<QualificationFundings> remove = new();

                foreach (var offerId in request.SelectedOfferIds)
                {
                    var offer = qualificationfundedOffers.FirstOrDefault(a => a.FundingOfferId == offerId);
                    if (offer == null) create.Add(new()
                    {
                        QualificationVersionId = request.QualificationVersionId,
                        FundingOfferId = offerId
                    });
                }

                foreach (var offer in qualificationfundedOffers)
                {
                    if (!request.SelectedOfferIds.Contains(offer.FundingOfferId))
                    {
                        remove.Add(offer);
                    }
                }

                if (remove.Count > 0) await _qualificationFundingsrepository.RemoveAsync(remove);
                if (create.Count > 0) await _qualificationFundingsrepository.CreateAsync(create);

                StringBuilder qualificationDiscussionHistoryNotes = new();
                qualificationDiscussionHistoryNotes.AppendLine("Feedback from DfE:");
                if (remove.Count > 0)
                {
                    qualificationDiscussionHistoryNotes.AppendLine("The following offers have been removed:");
                    qualificationDiscussionHistoryNotes.AppendLine();

                    foreach (var qf in remove)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine($"Offer: {fundingOffers.FirstOrDefault(a=>a.Id == qf.FundingOffer.Id)?.Name}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                        qualificationDiscussionHistoryNotes.AppendLine();
                    }
                }
                if (create.Count > 0)
                {
                    qualificationDiscussionHistoryNotes.AppendLine("The following offers have been selected:");
                    qualificationDiscussionHistoryNotes.AppendLine();

                    foreach (var qf in create)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine($"Offer: {fundingOffers.FirstOrDefault(a => a.Id == qf.FundingOffer.Id)?.Name}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                        qualificationDiscussionHistoryNotes.AppendLine();
                    }
                }

                if (remove.Count == 0 && create.Count == 0)
                {
                    qualificationDiscussionHistoryNotes.AppendLine("No Changes to funding offers");
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