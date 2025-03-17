using MediatR;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetFeedbackForQualificationFundingByIdQueryHandler : IRequestHandler<GetFeedbackForQualificationFundingByIdQuery, BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>>
{
    private readonly IQualificationFundingFeedbackRepository _qualificationFundingFeedbackRepository;
    private readonly IQualificationFundingsRepository _qualificationFundingsRepository;

    public GetFeedbackForQualificationFundingByIdQueryHandler(IQualificationFundingFeedbackRepository qualificationFundingFeedbackRepository, IQualificationFundingsRepository qualificationFundingsRepository)
    {
        _qualificationFundingFeedbackRepository = qualificationFundingFeedbackRepository;
        _qualificationFundingsRepository = qualificationFundingsRepository;
    }

    public async Task<BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>> Handle(GetFeedbackForQualificationFundingByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetFeedbackForQualificationFundingByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _qualificationFundingFeedbackRepository.GetQualificationFundingFeedbackDetailsByIdAsync(request.QualificationVersionId);
            if (result == null)
            {
                var qualificationFundingFeedback = new QualificationFundingFeedbacks
                {
                    QualificationVersionId = request.QualificationVersionId,
                };
                result = await _qualificationFundingFeedbackRepository.CreateAsync(qualificationFundingFeedback);
            }
            var qualificationFundings = await _qualificationFundingsRepository.GetByIdAsync(request.QualificationVersionId);

            // Convert qualificationFundings to the required type
            var qualificationFundedOffers = qualificationFundings.Select(qf => new GetFeedbackForQualificationFundingByIdQueryResponse.QualificationFunding
            {
                Id = qf.Id,
                FundingOfferId = qf.FundingOfferId,
                FundedOfferName = qf.FundingOffer.Name,
                StartDate = qf.StartDate,
                EndDate = qf.EndDate,
                Comments = qf.Comments
            }).ToList();

            response.Value = GetFeedbackForQualificationFundingByIdQueryResponse.Map(result, qualificationFundedOffers);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.InnerException = ex;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}