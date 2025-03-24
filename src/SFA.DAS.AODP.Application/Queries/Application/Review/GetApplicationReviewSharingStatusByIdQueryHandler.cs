using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationReviewSharingStatusByIdQueryHandler : IRequestHandler<GetApplicationReviewSharingStatusByIdQuery, BaseMediatrResponse<GetApplicationReviewSharingStatusByIdQueryResponse>>
    {
        private readonly IApplicationReviewRepository _applicationRepository;

        public GetApplicationReviewSharingStatusByIdQueryHandler(IApplicationReviewRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<BaseMediatrResponse<GetApplicationReviewSharingStatusByIdQueryResponse>> Handle(GetApplicationReviewSharingStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetApplicationReviewSharingStatusByIdQueryResponse>();
            response.Success = false;
            try
            {
                var result = await _applicationRepository.GetByIdAsync(request.ApplicationReviewId);
                response.Value = new()
                {
                    ApplicationId = result.ApplicationId,
                    SharedWithOfqual = result.SharedWithOfqual,
                    SharedWithSkillsEngland = result.SharedWithSkillsEngland,
                };
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
}