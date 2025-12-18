using MediatR;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Search;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetMatchingQualificationsQueryHandler : IRequestHandler<GetMatchingQualificationsQuery, BaseMediatrResponse<GetMatchingQualificationsQueryResponse>>
    {
        private readonly IQualificationsSearchService _qualificationsSearchService;

        public GetMatchingQualificationsQueryHandler(IQualificationsSearchService qualificationsSearchService)
        {
            _qualificationsSearchService = qualificationsSearchService;
        }
        public async Task<BaseMediatrResponse<GetMatchingQualificationsQueryResponse>> Handle(GetMatchingQualificationsQuery request, CancellationToken cancellationToken)
        {

            var response = new BaseMediatrResponse<GetMatchingQualificationsQueryResponse>();
            try
            {
                var qualifications = _qualificationsSearchService.SearchQualificationsByKeywordAsync(request.SearchTerm);

                response .Value = new GetMatchingQualificationsQueryResponse
                {
                    Qualifications = qualifications.Select(q => new GetMatchingQualificationsQueryItem
                    {
                        Id = q.Id,
                        Qan = q.Qan,
                        QualificationName = q.QualificationName
                    }).ToList()
                };
                response.Success = true;
            }
            catch (RecordWithNameNotFoundException ex)
            {
                response.Success = false;
                response.ErrorMessage = $"No qualifications were found matching search term: {request.SearchTerm}";
                response.ErrorCode = "NO_MATCHES";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
            }

            return response;
        }
    }
}