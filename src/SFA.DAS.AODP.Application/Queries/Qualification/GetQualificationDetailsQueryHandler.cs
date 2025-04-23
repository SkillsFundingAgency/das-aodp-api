using MediatR;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetQualificationDetailsQueryHandler : IRequestHandler<GetQualificationDetailsQuery, BaseMediatrResponse<GetQualificationDetailsQueryResponse>>
    {
        private readonly IQualificationDetailsRepository _repository;

        public GetQualificationDetailsQueryHandler(IQualificationDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetQualificationDetailsQueryResponse>> Handle(GetQualificationDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetQualificationDetailsQueryResponse>();
            try
            {
                var qualification = await _repository.GetQualificationDetailsByIdAsync(request.QualificationReference);
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


