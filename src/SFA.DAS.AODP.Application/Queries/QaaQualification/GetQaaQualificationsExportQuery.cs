using MediatR;

namespace SFA.DAS.AODP.Application.Queries.QaaQualification;

public class GetQaaQualificationsExportQuery : IRequest<BaseMediatrResponse<GetQaaQualificationsExportQueryResponse>>
{
    public string CurrentUsername { get; set; } = string.Empty;
}
