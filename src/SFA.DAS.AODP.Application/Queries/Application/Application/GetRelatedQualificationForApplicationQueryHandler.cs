using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.Qualification;

public class GetRelatedQualificationForApplicationQueryHandler : IRequestHandler<GetRelatedQualificationForApplicationQuery, BaseMediatrResponse<GetRelatedQualificationForApplicationQueryResponse>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IQualificationDetailsRepository _qualificationsRepository;

    public GetRelatedQualificationForApplicationQueryHandler(IApplicationRepository applicationRepository, IQualificationDetailsRepository qualificationsRepository)
    {
        _applicationRepository = applicationRepository;
        _qualificationsRepository = qualificationsRepository;
    }

    public async Task<BaseMediatrResponse<GetRelatedQualificationForApplicationQueryResponse>> Handle(GetRelatedQualificationForApplicationQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetRelatedQualificationForApplicationQueryResponse>();
        response.Success = false;
        try
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (string.IsNullOrWhiteSpace(application.QualificationNumber)) throw new Exception("No QAN has been provided for the application");

            var qualification = await _qualificationsRepository.GetQualificationDetailsByIdAsync(application.QualificationNumber);
            response.Value = new()
            {
                Qan = qualification.Qualification.Qan,
                Status = qualification.ProcessStatus?.Name,
                Name = qualification.Name
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