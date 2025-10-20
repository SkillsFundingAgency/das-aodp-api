using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Infrastructure;
using SFA.DAS.AODP.Models.Settings;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text;
namespace SFA.DAS.AODP.Application.Queries.Qualifications;


public class GetQualificationOutputFileQueryHandler : IRequestHandler<GetQualificationOutputFileQuery, BaseMediatrResponse<GetQualificationOutputFileResponse>>
{
    private readonly IQualificationOutputFileRepository _outputFileRepository;
    private readonly IQualificationOutputFileLogRepository _outputFileLogRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly OutputFileBlobStorageSettings _storageSettings;

    public GetQualificationOutputFileQueryHandler(IQualificationOutputFileRepository outputFileRepository, IQualificationOutputFileLogRepository outputFileLogRepository, IBlobStorageService blobStorageService, OutputFileBlobStorageSettings blobStorageSettings )
    {
        _outputFileRepository = outputFileRepository;
        _outputFileLogRepository = outputFileLogRepository;
        _blobStorageService = blobStorageService;
        _storageSettings = blobStorageSettings;
    }

    public async Task<BaseMediatrResponse<GetQualificationOutputFileResponse>> Handle(
    GetQualificationOutputFileQuery request,
    CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQualificationOutputFileResponse>();

        try
        {
            var qualifications = await _outputFileRepository.GetQualificationOutputFile();
            if (qualifications == null || !qualifications.Any())
            {
                response.Success = false;
                response.ErrorMessage = "No qualifications found for output file.";
                return response;
            }

            var active = qualifications.Where(q => GetMaxFundingEndDate(q) > DateTime.UtcNow.Date);
            var archived = qualifications.Where(q => GetMaxFundingEndDate(q) <= DateTime.UtcNow.Date);

            var currentDate = DateTime.UtcNow.ToString("yy-MM-dd");

            var approvedCsvFileName = $"{currentDate}-AOdPApprovedOutputFile.csv";
            var archivedCsvFileName = $"{currentDate}-AOdPArchivedOutputFile.csv";

            var approvedCsvFileBytes = await BuildCsvBytesAsync(active);
            var archivedCsvFileBytes = await BuildCsvBytesAsync(archived);

            using (var approvedCsvStream = new MemoryStream(approvedCsvFileBytes, writable: false))
            {
                await _blobStorageService.UploadFileAsync(
                    containerName: _storageSettings.ContainerName,
                    fileName: approvedCsvFileName,
                    content: approvedCsvStream,
                    contentType: "text/csv",
                    cancellationToken: cancellationToken);
            }

            using (var archivedCsvStream = new MemoryStream(archivedCsvFileBytes, writable: false))
            {
                await _blobStorageService.UploadFileAsync(
                    containerName: _storageSettings.ContainerName,
                    fileName: archivedCsvFileName,
                    content: archivedCsvStream,
                    contentType: "text/csv",
                    cancellationToken: cancellationToken);
            }

            byte[] zipBytes;
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true, Encoding.UTF8))
                {
                    var approvedZipEntry = zip.CreateEntry(approvedCsvFileName);
                    await using (var approvedZipEntryStream = approvedZipEntry.Open())
                    {
                        await approvedZipEntryStream.WriteAsync(approvedCsvFileBytes, cancellationToken);
                    }

                    var archivedZipEntry = zip.CreateEntry(archivedCsvFileName);
                    await using (var archivedZipEntryStream = archivedZipEntry.Open())
                    {
                        await archivedZipEntryStream.WriteAsync(archivedCsvFileBytes, cancellationToken);
                    }
                }
                zipBytes = ms.ToArray();
            }

            var history = new QualificationOutputFileLog
            {
                Id = Guid.NewGuid(),
                UserDisplayName = request.CurrentUsername, 
                Timestamp = DateTime.UtcNow,
                ApprovedFileName = approvedCsvFileName,
                ArchivedFileName = archivedCsvFileName,

            };
            await _outputFileLogRepository.CreateAsync(history, cancellationToken);

            var zipFileName = $"{currentDate}_qualifications_export.zip";

            response.Success = true;
            response.Value = new GetQualificationOutputFileResponse
            {
                FileName = zipFileName,
                ZipFileContent = zipBytes
            };
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            return response;
        }
    }
    private static DateTime? GetMaxFundingEndDate(QualificationOutputFile q)
    {
        DateTime? max = null;

        // check each possible funding end date; take the latest non-null
        void Consider(DateTime? candidate)
        {
            if (candidate.HasValue && (!max.HasValue || candidate > max))
                max = candidate;
        }

        Consider(q.Age1416_FundingApprovalEndDate);
        Consider(q.Age1619_FundingApprovalEndDate);
        Consider(q.LocalFlexibilities_FundingApprovalEndDate);
        Consider(q.LegalEntitlementL2L3_FundingApprovalEndDate);
        Consider(q.LegalEntitlementEnglishandMaths_FundingApprovalEndDate);
        Consider(q.DigitalEntitlement_FundingApprovalEndDate);
        Consider(q.LifelongLearningEntitlement_FundingApprovalEndDate);
        Consider(q.AdvancedLearnerLoans_FundingApprovalEndDate);
        Consider(q.FreeCoursesForJobs_FundingApprovalEndDate);

        return max;
    }

    private static string Csv(object? value)
    {
        if (value == null)
            return "";

        switch (value)
        {
            case string s:
                if (string.Equals(s, "NULL", StringComparison.OrdinalIgnoreCase))
                    return ""; // normalize literal "NULL"
                var needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
                return needsQuotes ? $"\"{s.Replace("\"", "\"\"")}\"" : s;

            case DateTime dt:
                return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            case bool b:
                return b ? "1" : "0";

            case IFormattable formattable: // covers int, decimal, double, etc.
                return formattable.ToString(null, CultureInfo.InvariantCulture);

            default:
                var str = value.ToString();
                if (string.IsNullOrEmpty(str)) return "";
                var needsQuotes2 = str.Contains(',') || str.Contains('"') || str.Contains('\n') || str.Contains('\r');
                return needsQuotes2 ? $"\"{str.Replace("\"", "\"\"")}\"" : str;
        }
    }

    private static readonly string CsvHeader = string.Join(",", new[]
    {
        "DateOfOfqualDataSnapshot",
        "QualificationName",
        "AwardingOrganisation",
        "QualificationNumber",
        "Level",
        "QualificationType",
        "Subcategory",
        "SectorSubjectArea",
        "Status",
        "Age1416_FundingAvailable",
        "Age1416_FundingApprovalStartDate",
        "Age1416_FundingApprovalEndDate",
        "Age1416_Notes",
        "Age1619_FundingAvailable",
        "Age1619_FundingApprovalStartDate",
        "Age1619_FundingApprovalEndDate",
        "Age1619_Notes",
        "LocalFlexibilities_FundingAvailable",
        "LocalFlexibilities_FundingApprovalStartDate",
        "LocalFlexibilities_FundingApprovalEndDate",
        "LocalFlexibilities_Notes",
        "LegalEntitlementL2L3_FundingAvailable",
        "LegalEntitlementL2L3_FundingApprovalStartDate",
        "LegalEntitlementL2L3_FundingApprovalEndDate",
        "LegalEntitlementL2L3_Notes",
        "LegalEntitlementEnglishandMaths_FundingAvailable",
        "LegalEntitlementEnglishandMaths_FundingApprovalStartDate",
        "LegalEntitlementEnglishandMaths_FundingApprovalEndDate",
        "LegalEntitlementEnglishandMaths_Notes",
        "DigitalEntitlement_FundingAvailable",
        "DigitalEntitlement_FundingApprovalStartDate",
        "DigitalEntitlement_FundingApprovalEndDate",
        "DigitalEntitlement_Notes",
        "LifelongLearningEntitlement_FundingAvailable",
        "LifelongLearningEntitlement_FundingApprovalStartDate",
        "LifelongLearningEntitlement_FundingApprovalEndDate",
        "LifelongLearningEntitlement_Notes",
        "AdvancedLearnerLoans_FundingAvailable",
        "AdvancedLearnerLoans_FundingApprovalStartDate",
        "AdvancedLearnerLoans_FundingApprovalEndDate",
        "AdvancedLearnerLoans_Notes",
        "AwardingOrganisationURL",
        "FreeCoursesForJobs_FundingAvailable",
        "FreeCoursesForJobs_FundingApprovalStartDate",
        "FreeCoursesForJobs_FundingApprovalEndDate",
        "FreeCoursesForJobs_Notes"
    });

    private static async Task<byte[]> BuildCsvBytesAsync(IEnumerable<QualificationOutputFile> rows)
    {
        using var ms = new MemoryStream();
        await WriteCsvAsync(ms, rows); 
        await ms.FlushAsync();
        return ms.ToArray();
    }
    private static async Task WriteCsvAsync(Stream output, IEnumerable<QualificationOutputFile> rows)
    {
        await using var w = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);
        await w.WriteLineAsync(CsvHeader);

        foreach (var q in rows)
        {
            var line = string.Join(",", new[]
            {
                Csv(q.DateOfOfqualDataSnapshot),
                Csv(q.QualificationName),
                Csv(q.AwardingOrganisation),
                Csv(q.QualificationNumber),
                Csv(q.Level),
                Csv(q.QualificationType),
                Csv(q.Subcategory),
                Csv(q.SectorSubjectArea),
                Csv(q.Status),

                Csv(q.Age1416_FundingAvailable?.ToString()),
                Csv(q.Age1416_FundingApprovalStartDate),
                Csv(q.Age1416_FundingApprovalEndDate),
                Csv(q.Age1416_Notes),

                Csv(q.Age1619_FundingAvailable?.ToString()),
                Csv(q.Age1619_FundingApprovalStartDate),
                Csv(q.Age1619_FundingApprovalEndDate),
                Csv(q.Age1619_Notes),

                Csv(q.LocalFlexibilities_FundingAvailable?.ToString()),
                Csv(q.LocalFlexibilities_FundingApprovalStartDate),
                Csv(q.LocalFlexibilities_FundingApprovalEndDate),
                Csv(q.LocalFlexibilities_Notes),

                Csv(q.LegalEntitlementL2L3_FundingAvailable?.ToString()),
                Csv(q.LegalEntitlementL2L3_FundingApprovalStartDate),
                Csv(q.LegalEntitlementL2L3_FundingApprovalEndDate),
                Csv(q.LegalEntitlementL2L3_Notes),

                Csv(q.LegalEntitlementEnglishandMaths_FundingAvailable?.ToString()),
                Csv(q.LegalEntitlementEnglishandMaths_FundingApprovalStartDate),
                Csv(q.LegalEntitlementEnglishandMaths_FundingApprovalEndDate),
                Csv(q.LegalEntitlementEnglishandMaths_Notes),

                Csv(q.DigitalEntitlement_FundingAvailable?.ToString()),
                Csv(q.DigitalEntitlement_FundingApprovalStartDate),
                Csv(q.DigitalEntitlement_FundingApprovalEndDate),
                Csv(q.DigitalEntitlement_Notes),

                Csv(q.LifelongLearningEntitlement_FundingAvailable?.ToString()),
                Csv(q.LifelongLearningEntitlement_FundingApprovalStartDate),
                Csv(q.LifelongLearningEntitlement_FundingApprovalEndDate),
                Csv(q.LifelongLearningEntitlement_Notes),

                Csv(q.AdvancedLearnerLoans_FundingAvailable?.ToString()),
                Csv(q.AdvancedLearnerLoans_FundingApprovalStartDate),
                Csv(q.AdvancedLearnerLoans_FundingApprovalEndDate),
                Csv(q.AdvancedLearnerLoans_Notes),

                Csv(q.AwardingOrganisationURL),

                Csv(q.FreeCoursesForJobs_FundingAvailable?.ToString()),
                Csv(q.FreeCoursesForJobs_FundingApprovalStartDate),
                Csv(q.FreeCoursesForJobs_FundingApprovalEndDate),
                Csv(q.FreeCoursesForJobs_Notes),
            });

            await w.WriteLineAsync(line);
        }

        await w.FlushAsync();
        
    }
}


