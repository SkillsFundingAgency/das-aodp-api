using MediatR;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetQualificationDetailWithVersionsQueryHandler : IRequestHandler<GetQualificationDetailWithVersionsQuery, BaseMediatrResponse<GetQualificationDetailsQueryResponse>>
    {
        private readonly IQualificationDetailsRepository _repository;

        public GetQualificationDetailWithVersionsQueryHandler(IQualificationDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetQualificationDetailsQueryResponse>> Handle(GetQualificationDetailWithVersionsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetQualificationDetailsQueryResponse>();
            response.Success = false;
            try
            {
                var qualification = await _repository.GetQualificationDetailWithVersions(request.QualificationReference);
                response.Value = qualification;
                response.Success = true;
            }
            catch (RecordWithNameNotFoundException ex)
            {
                response.Success = false;
                response.ErrorMessage = $"No details found for qualification reference: {request.QualificationReference}";
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


