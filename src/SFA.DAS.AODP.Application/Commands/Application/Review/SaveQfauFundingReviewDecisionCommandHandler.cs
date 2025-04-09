using MediatR;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Application;
using System.Text;
using ProcessStatus = SFA.DAS.AODP.Data.Enum.ProcessStatus;

namespace SFA.DAS.AODP.Application.Commands.Application.Review
{
    public class SaveQfauFundingReviewDecisionCommandHandler : IRequestHandler<SaveQfauFundingReviewDecisionCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private Guid NoActionRequiredActionTypeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private Guid ApprovedProcessStatusId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        private Guid RejectedProcessStatusId = Guid.Parse("00000000-0000-0000-0000-000000000005");
        private readonly IApplicationReviewFeedbackRepository _reviewRepository;
        private readonly IMediator _mediator;
        private readonly IQualificationDetailsRepository _qualificationDetailsRepository;
        private readonly IQualificationFundingsRepository _qualificationFundingsrepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        private readonly IQualificationsRepository _qualificationsRepository;

        public SaveQfauFundingReviewDecisionCommandHandler(IApplicationReviewFeedbackRepository repository, IMediator mediator, IQualificationDetailsRepository qualificationDetailsRepository, IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository, IQualificationFundingsRepository qualificationFundingsRepository, IQualificationsRepository qualificationsRepository)
        {
            _reviewRepository = repository;
            _mediator = mediator;
            _qualificationDetailsRepository = qualificationDetailsRepository;
            _qualificationDiscussionHistoryRepository = qualificationDiscussionHistoryRepository;
            _qualificationFundingsrepository = qualificationFundingsRepository;
            _qualificationsRepository = qualificationsRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(SaveQfauFundingReviewDecisionCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var review = await _reviewRepository.GeyByReviewIdAndUserType(request.ApplicationReviewId, UserType.Qfau);
                var qan = review.ApplicationReview?.Application?.QualificationNumber;

                if (review.Status == ApplicationStatus.Approved.ToString() && string.IsNullOrWhiteSpace(qan))
                {
                    throw new InvalidOperationException("No QAN has been provided for the application.");
                }

                if (!string.IsNullOrWhiteSpace(qan))
                {
                    var qualVersion = await _qualificationDetailsRepository.GetQualificationDetailsByIdAsync(qan);

                    if (qualVersion == null || qualVersion.ProcessStatus.Name != ProcessStatus.DecisionRequired)
                    {
                        throw new InvalidOperationException("The current qualification status is not valid to confirm the decision.");
                    }

                    await UpdateQualificationOffersAsync(review, qualVersion, request.SentByName);
                }


                if (review.Status == ApplicationStatus.NotApproved.ToString() && review?.ApplicationReview?.ApplicationReviewFundings?.Any() == true)
                {
                    throw new InvalidOperationException("The application has been rejected for funding but has approved offers associated.");
                }

                await AddApplicationMessage(request, review);

                review.LatestCommunicatedToAwardingOrganisation = true;
                review.ApplicationReview.Application.Status = review.Status;
                await _reviewRepository.UpdateAsync(review);

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

        private async Task AddApplicationMessage(SaveQfauFundingReviewDecisionCommand request, ApplicationReviewFeedback review)
        {
            var msg = new CreateApplicationMessageCommand()
            {
                ApplicationId = review.ApplicationReview.ApplicationId,
                MessageType = MessageType.AoInformedOfDecision.ToString(),
                UserType = UserType.Qfau.ToString(),
                SentByEmail = request.SentByEmail,
                SentByName = request.SentByName,
            };

            StringBuilder msgText = new();

            msgText.AppendLine("Feedback from DfE:");
            msgText.AppendLine(review.Comments);
            msgText.AppendLine();
            if (review?.ApplicationReview?.ApplicationReviewFundings?.Any() == true)
            {
                msgText.AppendLine("The following offers have been approved:");
                msgText.AppendLine();

                foreach (var offer in review.ApplicationReview.ApplicationReviewFundings)
                {
                    msgText.AppendLine($"Offer: {offer.FundingOffer.Name}");
                    msgText.AppendLine($"Start date: {offer.StartDate}");
                    msgText.AppendLine($"End date: {offer.EndDate}");
                    msgText.AppendLine($"Comments: {offer.Comments}");
                    msgText.AppendLine();
                }
            }

            msg.MessageText = msgText.ToString();

            var msgResult = await _mediator.Send(msg);
            if (!msgResult.Success) throw new Exception(msgResult.ErrorMessage, msgResult.InnerException);
        }

        private async Task UpdateQualificationOffersAsync(ApplicationReviewFeedback review, QualificationVersions qualVersion, string sentByName)
        {
            // get the current state before the update for discussion history notes
            var qualificationfundedOffers = await _qualificationFundingsrepository.GetByIdAsync(qualVersion.Id);

            var updateOutcome = await _mediator.Send(new SaveQualificationsFundingOffersOutcomeCommand()
            {
                UpdateDiscussionHistory = false,
                ActionTypeId = NoActionRequiredActionTypeId,
                Approved = review.Status == ApplicationStatus.Approved.ToString(),
                QualificationId = qualVersion.QualificationId,
                QualificationVersionId = qualVersion.Id,
                Comments = review.Comments
            });
            if (updateOutcome.Success == false) throw new Exception(updateOutcome.ErrorMessage, updateOutcome.InnerException);

            var updateOffers = await _mediator.Send(new SaveQualificationsFundingOffersCommand()
            {
                UpdateDiscussionHistory = false,
                ActionTypeId = NoActionRequiredActionTypeId,
                QualificationId = qualVersion.QualificationId,
                QualificationVersionId = qualVersion.Id,
                SelectedOfferIds = review.ApplicationReview.ApplicationReviewFundings?.Select(review => review.FundingOfferId).ToList() ?? []
            });
            if (updateOffers.Success == false) throw new Exception(updateOffers.ErrorMessage, updateOffers.InnerException);


            var updateOfferDetails = await _mediator.Send(new SaveQualificationsFundingOffersDetailsCommand()
            {
                UpdateDiscussionHistory = false,
                ActionTypeId = NoActionRequiredActionTypeId,
                QualificationId = qualVersion.QualificationId,
                QualificationVersionId = qualVersion.Id,
                Details = review.ApplicationReview.ApplicationReviewFundings?.Select(review => new SaveQualificationsFundingOffersDetailsCommand.OfferFundingDetails()
                {
                    Comments = review.Comments,
                    StartDate = review.StartDate,
                    EndDate = review.EndDate,
                    FundingOfferId = review.FundingOfferId,
                }).ToList() ?? []
            });
            if (updateOfferDetails.Success == false) throw new Exception(updateOfferDetails.ErrorMessage, updateOfferDetails.InnerException);

            await _qualificationsRepository.UpdateQualificationStatus(qualVersion.Qualification.Qan, review.Status == ApplicationStatus.Approved.ToString() ? ApprovedProcessStatusId : RejectedProcessStatusId, qualVersion.Version);

            var applicationOfferIds = review.ApplicationReview.ApplicationReviewFundings?.Select(a => a.FundingOfferId) ?? [];
            List<QualificationFundings> removedOffers = qualificationfundedOffers.Where(q => !applicationOfferIds.Contains(q.FundingOfferId)).ToList();

            StringBuilder qualificationDiscussionHistoryNotes = new();
            if (removedOffers.Count > 0)
            {
                qualificationDiscussionHistoryNotes.AppendLine("The following offers have been removed:");
                qualificationDiscussionHistoryNotes.AppendLine();

                foreach (var qf in removedOffers)
                {
                    var fundingOffer = qf.FundingOffer;
                    if (fundingOffer != null)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine($"Offer: {fundingOffer.Name}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                        qualificationDiscussionHistoryNotes.AppendLine();
                    }
                }
            }
            if (review.ApplicationReview.ApplicationReviewFundings?.Count > 0)
            {
                qualificationDiscussionHistoryNotes.AppendLine("The following offers have been approved:");
                qualificationDiscussionHistoryNotes.AppendLine();

                foreach (var qf in review.ApplicationReview.ApplicationReviewFundings)
                {
                    var fundingOffer = qf.FundingOffer;
                    if (fundingOffer != null)
                    {
                        qualificationDiscussionHistoryNotes.AppendLine($"Offer: {fundingOffer.Name}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Start date: {qf.StartDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"End date: {qf.EndDate?.ToString("dd-MM-yyyy")}");
                        qualificationDiscussionHistoryNotes.AppendLine($"Comments: {qf.Comments}");
                        qualificationDiscussionHistoryNotes.AppendLine();
                    }
                }
            }


            await _qualificationDiscussionHistoryRepository.CreateAsync(new QualificationDiscussionHistory
            {
                QualificationId = qualVersion.QualificationId,
                UserDisplayName = sentByName,
                Notes = qualificationDiscussionHistoryNotes.ToString(),
                ActionTypeId = NoActionRequiredActionTypeId,
                Timestamp = DateTime.UtcNow,
                Title = $"{(review.Status == ApplicationStatus.Approved.ToString() ? "Approved" : "Rejected")} via Application:{review.ApplicationReview.Application.ReferenceId.ToString().PadLeft(6, '0')}"
            });
        }
    }
}