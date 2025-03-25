using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetQualificationVersionsForQualificationByReferenceQueryHandler : IRequestHandler<GetQualificationVersionsForQualificationByReferenceQuery, BaseMediatrResponse<GetQualificationVersionsForQualificationByReferenceQueryResponse>>
{
    private readonly IQualificationsRepository _qualificationsRepository;


    public GetQualificationVersionsForQualificationByReferenceQueryHandler(IQualificationsRepository qualificationsRepository)
    {
        _qualificationsRepository = qualificationsRepository;
    }

    public async Task<BaseMediatrResponse<GetQualificationVersionsForQualificationByReferenceQueryResponse>> Handle(GetQualificationVersionsForQualificationByReferenceQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQualificationVersionsForQualificationByReferenceQueryResponse>();
        response.Success = false;
        try
        {
            var qualification = await _qualificationsRepository.GetByIdAsync(request.QualificationReference);
            if (qualification == null)
            {
                response.ErrorMessage = "Qualification not found";
                return response;
            }
            response.Value = qualification;
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