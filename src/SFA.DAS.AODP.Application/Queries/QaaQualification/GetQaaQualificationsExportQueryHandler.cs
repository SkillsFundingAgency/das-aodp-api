using MediatR;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;
using System.Globalization;
using System.Text;

namespace SFA.DAS.AODP.Application.Queries.QaaQualification;

public class GetQaaQualificationsExportQueryHandler : IRequestHandler<GetQaaQualificationsExportQuery, BaseMediatrResponse<GetQaaQualificationsExportQueryResponse>>
{
    private readonly IQaaQualificationRepository _qaaQualificationRepository;
    private readonly IQaaQualificationDownloadLogRepository _downloadLogRepository;

    public const string NoQualificationsFound = "No QAA qualifications found for the export file.";
    public const string UnexpectedErrorGeneratingFile = "An unexpected error occurred while generating the QAA export file.";

    public GetQaaQualificationsExportQueryHandler(
        IQaaQualificationRepository qaaQualificationRepository,
        IQaaQualificationDownloadLogRepository downloadLogRepository)
    {
        _qaaQualificationRepository = qaaQualificationRepository;
        _downloadLogRepository = downloadLogRepository;
    }

    public async Task<BaseMediatrResponse<GetQaaQualificationsExportQueryResponse>> Handle(
        GetQaaQualificationsExportQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQaaQualificationsExportQueryResponse>();

        try
        {
            var qualifications = await _qaaQualificationRepository.GetAllAsync(cancellationToken);
            var qualificationsList = qualifications.ToList();

            if (qualificationsList.Count == 0)
            {
                response.Success = false;
                response.ErrorMessage = NoQualificationsFound;
                response.ErrorCode = ErrorCodes.NoData;
                return response;
            }

            var csvFileName = $"{DateTime.UtcNow:yyyy-MM-dd}-QaaQualifications.csv";
            var csvFileBytes = await BuildCsvBytesAsync(qualificationsList);

            await _downloadLogRepository.CreateAsync(new QaaQualificationDownloadLog
            {
                UserDisplayName = request.CurrentUsername,
                DownloadDate = DateTime.UtcNow,
                FileName = csvFileName
            }, cancellationToken);

            response.Success = true;
            response.Value = new GetQaaQualificationsExportQueryResponse
            {
                FileName = csvFileName,
                FileContent = csvFileBytes
            };
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = UnexpectedErrorGeneratingFile;
            response.ErrorCode = ErrorCodes.UnexpectedError;
            response.InnerException = ex;
            return response;
        }
    }

    private static string GetStatus(RegulatedQaaQualification q)
    {
        if (q.IsDiscontinued)
            return "Discontinued";

        if (q.LatestImportComparisonOutcome == QaaImportComparisonOutcome.New)
            return "New";

        if (q.LatestImportComparisonOutcome == QaaImportComparisonOutcome.LastDateForRegistrationChanged &&
            q.LastDateForRegistrationChangeType == QaaLastDateForRegistrationChangeType.Extended)
            return "Extended";

        return "Unchanged";
    }

    private static string Csv(object? value)
    {
        if (value == null)
            return "";

        switch (value)
        {
            case string s:
                var needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
                return needsQuotes ? $"\"{s.Replace("\"", "\"\"")}\"" : s;

            case DateTime dt:
                return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            case DateOnly d:
                return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            default:
                return value.ToString() ?? "";
        }
    }

    private static readonly string CsvHeader = string.Join(",", new[]
    {
        "Snapshot Date",
        "QualificationID",
        "Status",
        "Awarding Body Name",
        "Qualification Title",
        "SSA",
        "Start Date",
        "Certification End Date",
        "Discontinued Date",
        "Registration End Date"
    });

    private static async Task<byte[]> BuildCsvBytesAsync(IEnumerable<RegulatedQaaQualification> rows)
    {
        using var ms = new MemoryStream();
        await WriteCsvAsync(ms, rows);
        await ms.FlushAsync();
        return ms.ToArray();
    }

    private static async Task WriteCsvAsync(Stream output, IEnumerable<RegulatedQaaQualification> rows)
    {
        await using var w = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);
        await w.WriteLineAsync(CsvHeader);

        foreach (var q in rows)
        {
            var line = string.Join(",", new[]
            {
                Csv(q.DateOfDataSnapshot),
                Csv(q.AimCode),
                Csv(GetStatus(q)),
                Csv(q.AwardingBody),
                Csv(q.QualificationTitle),
                Csv(q.SectorSubjectArea?.Name),
                Csv(q.StartDate),
                Csv(q.LastDateForCertifications),
                Csv(q.DiscontinuedDate),
                Csv(q.LastDateForRegistration)
            });

            await w.WriteLineAsync(line);
        }

        await w.FlushAsync();
    }
}
