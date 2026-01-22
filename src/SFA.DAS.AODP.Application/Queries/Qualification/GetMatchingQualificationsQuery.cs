using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetMatchingQualificationsQuery : IRequest<BaseMediatrResponse<GetMatchingQualificationsQueryResponse>>
{
    public string? SearchTerm { get; set; }

    // Pagination
    public int? Skip { get; set; } = 0;
    public int? Take { get; set; } = 25;
}