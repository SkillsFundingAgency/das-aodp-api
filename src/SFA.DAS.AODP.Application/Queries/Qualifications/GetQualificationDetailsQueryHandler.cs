using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetQualificationDetailsQueryHandler : IRequestHandler<GetQualificationDetailsQuery, GetQualificationDetailsQueryResponse>
    {
        private readonly INewQualificationsRepository _repository;

        public GetQualificationDetailsQueryHandler(INewQualificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetQualificationDetailsQueryResponse> Handle(GetQualificationDetailsQuery request, CancellationToken cancellationToken)
        {
            var qualification = await _repository.GetQualificationDetailsByIdAsync(request.QualificationReference);
            if (qualification == null)
            {
                return new GetQualificationDetailsQueryResponse { Success = false };
            }

            return new GetQualificationDetailsQueryResponse
            {
                Success = true,
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
        }
    }
}
