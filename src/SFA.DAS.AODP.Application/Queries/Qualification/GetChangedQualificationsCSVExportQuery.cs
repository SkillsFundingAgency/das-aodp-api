using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualification
{
    public class GetChangedQualificationsCsvExportQuery : IRequest<BaseMediatrResponse<GetChangedQualificationsCsvExportResponse>>
    {
    }
}
