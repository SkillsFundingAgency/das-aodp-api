using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetChangedQualificationDetailsHandler : IRequestHandler<GetChangedQualificationDetailsQuery, BaseMediatrResponse<GetChangedQualificationDetailsResponse>>
    {
        private readonly IChangedQualificationsRepository _repository;

        public GetChangedQualificationDetailsHandler(IChangedQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetChangedQualificationDetailsResponse>> Handle(GetChangedQualificationDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetChangedQualificationDetailsResponse>();
            try
            {
                var qualification = await _repository.GetQualificationDetailsByIdAsync(request.QualificationReference);
                if (qualification != null)
                {
                    response.Value = new GetChangedQualificationDetailsResponse
                    {
                        Id = qualification.Id,
                        StatusId=qualification.StatusId,
                        Status = qualification.Status,
                        Priority = qualification.Priority,
                        Changes = qualification.Changes,
                        QualificationReference = qualification.QualificationReference,
                        AwardingOrganisation = qualification.AwardingOrganisation,
                        Title = qualification.Title,
                        QualificationType = qualification.QualificationType,
                        Level = qualification.Level,
                        ProposedChanges = qualification.ProposedChanges,
                        AgeGroup = qualification.AgeGroup,
                        Category = qualification.Category,
                        Subject = qualification.Subject,
                        SectorSubjectArea = qualification.SectorSubjectArea,
                        Comments = qualification.Comments
                    };
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = $"No details found for qualification reference: {request.QualificationReference}";
                }
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


