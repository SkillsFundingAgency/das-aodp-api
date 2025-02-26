using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetNewQualificationsCsvExportQuery : IRequest<BaseMediatrResponse<GetNewQualificationsCsvExportResponse>>
    {
    }
}
