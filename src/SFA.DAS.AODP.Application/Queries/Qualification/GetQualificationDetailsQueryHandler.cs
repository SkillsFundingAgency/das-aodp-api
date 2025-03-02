using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetQualificationDetailsQueryHandler : IRequestHandler<GetQualificationDetailsQuery, BaseMediatrResponse<GetQualificationDetailsQueryResponse>>
    {
        private readonly IQualificationsRepository _repository;

        public GetQualificationDetailsQueryHandler(IQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetQualificationDetailsQueryResponse>> Handle(GetQualificationDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetQualificationDetailsQueryResponse>();
            try
            {
                var qualification = await _repository.GetQualificationDetailsByIdAsync(request.QualificationReference);
                if (qualification != null)
                {
                    response.Value = new GetQualificationDetailsQueryResponse
                    {
                        Id = qualification.Id,
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


