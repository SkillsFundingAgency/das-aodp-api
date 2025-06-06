﻿using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetFeedbackForQualificationFundingByIdQueryResponse
{
    public Guid Id { get; set; }
    public Guid QualificationVersionId { get; set; }
    public string? QualificationReference { get; set; }
    public bool? Approved { get; set; }
    public string? Comments { get; set; }

    public List<QualificationFunding> QualificationFundedOffers { get; set; } = new();

    public class QualificationFunding
    {
        public Guid Id { get; set; }
        public Guid FundingOfferId { get; set; }
        public string FundedOfferName { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Comments { get; set; }
    }

    public static implicit operator GetFeedbackForQualificationFundingByIdQueryResponse(QualificationFundingFeedbacks feedback)
    {
        GetFeedbackForQualificationFundingByIdQueryResponse model = new()
        {
            Id = feedback.Id,
            QualificationVersionId = feedback.QualificationVersionId,
            Comments = feedback.Comments,
            Approved = feedback?.Approved,
        };
        return model;
    }

    public static GetFeedbackForQualificationFundingByIdQueryResponse Map(QualificationFundingFeedbacks feedback, List<QualificationFunding> qualificationFundedOffers)
    {
        GetFeedbackForQualificationFundingByIdQueryResponse model = new()
        {
            Id = feedback.Id,
            QualificationVersionId = feedback.QualificationVersionId,
            QualificationReference = feedback.QualificationVersion?.Qualification?.Qan,
            Comments = feedback?.Comments,
            Approved = feedback?.Approved,
        };

        foreach (var funding in qualificationFundedOffers ?? [])
        {
            model.QualificationFundedOffers.Add(new()
            {
                Id = funding.Id,
                FundedOfferName = funding.FundedOfferName,
                FundingOfferId = funding.FundingOfferId,
                Comments = funding.Comments,
                EndDate = funding.EndDate,
                StartDate = funding?.StartDate,
            });
        }

        return model;
    }
}