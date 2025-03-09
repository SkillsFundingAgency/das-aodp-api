using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Review
{
    public class GetApplicationForReviewByIdQueryHandler : IRequestHandler<GetApplicationForReviewByIdQuery, BaseMediatrResponse<GetApplicationForReviewByIdQueryResponse>>
    {
        private readonly IApplicationReviewRepository _applicationRepository;

        public GetApplicationForReviewByIdQueryHandler(IApplicationReviewRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<BaseMediatrResponse<GetApplicationForReviewByIdQueryResponse>> Handle(GetApplicationForReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetApplicationForReviewByIdQueryResponse>();
            response.Success = false;
            try
            {
                var result = await _applicationRepository.GetApplicationForReviewByReviewIdAsync(request.ApplicationReviewId);
                response.Value = result;
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