using MediatR;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;

namespace SFA.DAS.AODP.Application.Queries.QaaQualification;

public class GetQaaDownloadSummaryQueryHandler : IRequestHandler<GetQaaDownloadSummaryQuery, BaseMediatrResponse<GetQaaDownloadSummaryQueryResponse>>
{
    private readonly IQaaQualificationRepository _qaaQualificationRepository;
    private readonly IQaaQualificationDownloadLogRepository _downloadLogRepository;

    public GetQaaDownloadSummaryQueryHandler(
        IQaaQualificationRepository qaaQualificationRepository,
        IQaaQualificationDownloadLogRepository downloadLogRepository)
    {
        _qaaQualificationRepository = qaaQualificationRepository;
        _downloadLogRepository = downloadLogRepository;
    }

    public async Task<BaseMediatrResponse<GetQaaDownloadSummaryQueryResponse>> Handle(GetQaaDownloadSummaryQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQaaDownloadSummaryQueryResponse>();

        try
        {
            var counts = await _qaaQualificationRepository.GetSummaryCountsAsync(cancellationToken);
            var history = await _downloadLogRepository.ListAsync(take: 10, ct: cancellationToken);

            response.Success = true;
            response.Value = new GetQaaDownloadSummaryQueryResponse
            {
                DataLastImportedDate = counts.DataLastImportedDate,
                MostRecentDownloadDate = history.Count > 0 ? history.Max(x => x.DownloadDate) : null,
                NewQualificationsCount = counts.NewCount,
                ExtendedQualificationsCount = counts.ExtendedCount,
                DiscontinuedQualificationsCount = counts.DiscontinuedCount,
                DownloadHistory = history.Select(x => new GetQaaDownloadSummaryQueryResponse.QaaDownloadLog
                {
                    UserDisplayName = x.UserDisplayName,
                    DownloadDate = x.DownloadDate
                }).ToList()
            };
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
