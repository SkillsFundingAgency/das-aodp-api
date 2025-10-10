using MediatR;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System;
using System.Globalization;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using System.Text;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;


public class GetQualificationExportFileQueryHandler : IRequestHandler<GetQualificationExportFileQuery, BaseMediatrResponse<GetQualificationExportFileResponse>>
{
    private readonly IExportQualificationsRepository _repository;

    public GetQualificationExportFileQueryHandler(IExportQualificationsRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseMediatrResponse<GetQualificationExportFileResponse>> Handle(GetQualificationExportFileQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQualificationExportFileResponse>();
        try
        {
            var qualifications = QualificationExportFactory.CreateDummyQualifications();// await _repository.GetQualificationExport();
            if (qualifications == null || !qualifications.Any())
            {
                response.Success = false;
                response.ErrorMessage = "No qualifications found for export.";
            }
            else
            {

                foreach (var item in qualifications)
                {
                    item.Archived = GetMaxFundingEndDate(item) < DateTime.UtcNow.Date;
                }

                var active = qualifications.Where(q => !q.Archived);
                var archived = qualifications.Where(q => q.Archived);

                var currentDate = DateTime.Now.ToString("yy-MM-dd");

                byte[] zipBytes;
                using (var ms = new MemoryStream())
                {
                    using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true, Encoding.UTF8))
                    {
                        
                        var e1 = zip.CreateEntry($"{currentDate}-AOdPApprovedOutputFile.csv");
                        await using (var s1 = e1.Open())
                        {
                            await WriteCsvAsync(s1, active);
                        }

                        var e2 = zip.CreateEntry($"{currentDate}-AOdPArchivedOutputFile.csv");
                        await using (var s2 = e2.Open())
                        {
                            await WriteCsvAsync(s2, archived);
                        }
                    }

                    zipBytes = ms.ToArray();
                }

                response.Success = true;
                response.Value = new GetQualificationExportFileResponse
                {
                    FileName = $"{currentDate}_qualifications_export.zip",
                    ZipFileContent = zipBytes
                };
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }
        return response;
    }

    private static DateTime? GetMaxFundingEndDate(QualificationExport q)
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

    private static string Csv(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var needs = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
        return needs ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
    }
    private static string Csv(DateTime? dt) =>
        dt.HasValue ? dt.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";

    private static readonly string CsvHeader = string.Join(",", new[]
    {
        "DateOfOfqualDataSnapshot","QualificationName","AwardingOrganisation","QualificationNumber","Level",
        "QualificationType","Subcategory","SectorSubjectArea","Status",
        "Age1416_FundingAvailable","Age1416_FundingApprovalStartDate","Age1416_FundingApprovalEndDate","Age1416_Notes",
        "Age1619_FundingAvailable","Age1619_FundingApprovalStartDate","Age1619_FundingApprovalEndDate","Age1619_Notes",
        "LocalFlexibilities_FundingAvailable","LocalFlexibilities_FundingApprovalStartDate","LocalFlexibilities_FundingApprovalEndDate","LocalFlexibilities_Notes",
        "LegalEntitlementL2L3_FundingAvailable","LegalEntitlementL2L3_FundingApprovalStartDate","LegalEntitlementL2L3_FundingApprovalEndDate","LegalEntitlementL2L3_Notes",
        "LegalEntitlementEnglishandMaths_FundingAvailable","LegalEntitlementEnglishandMaths_FundingApprovalStartDate","LegalEntitlementEnglishandMaths_FundingApprovalEndDate","LegalEntitlementEnglishandMaths_Notes",
        "DigitalEntitlement_FundingAvailable","DigitalEntitlement_FundingApprovalStartDate","DigitalEntitlement_FundingApprovalEndDate","DigitalEntitlement_Notes",
        "LifelongLearningEntitlement_FundingAvailable","LifelongLearningEntitlement_FundingApprovalStartDate","LifelongLearningEntitlement_FundingApprovalEndDate","LifelongLearningEntitlement_Notes",
        "AdvancedLearnerLoans_FundingAvailable","AdvancedLearnerLoans_FundingApprovalStartDate","AdvancedLearnerLoans_FundingApprovalEndDate","AdvancedLearnerLoans_Notes",
        "AwardingOrganisationURL",
        "FreeCoursesForJobs_FundingAvailable","FreeCoursesForJobs_FundingApprovalStartDate","FreeCoursesForJobs_FundingApprovalEndDate","FreeCoursesForJobs_Notes"
    });
    private static async Task WriteCsvAsync(Stream output, IEnumerable<QualificationExport> rows)
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
            Csv(q.Level?.ToString(CultureInfo.InvariantCulture)),
            Csv(q.QualificationType),
            Csv(q.Subcategory),
            Csv(q.SectorSubjectArea),
            Csv(q.Status),

            Csv(q.Age1416_FundingAvailable?.ToString()),
            Csv(q.Age1416_FundingApprovalStartDate),
            Csv(q.Age1416_FundingApprovalEndDate),
            Csv("Missing from view" /*q.Age1416_Notes*/),

            Csv(q.Age1619_FundingAvailable?.ToString()),
            Csv(q.Age1619_FundingApprovalStartDate),
            Csv(q.Age1619_FundingApprovalEndDate),
            Csv("Missing from view" /*q.Age1619_Notes*/),

            Csv(q.LocalFlexibilities_FundingAvailable?.ToString()),
            Csv(q.LocalFlexibilities_FundingApprovalStartDate),
            Csv(q.LocalFlexibilities_FundingApprovalEndDate),
            Csv("Missing from view" /*q.LocalFlexibilities_Notes*/),

            Csv(q.LegalEntitlementL2L3_FundingAvailable?.ToString()),
            Csv(q.LegalEntitlementL2L3_FundingApprovalStartDate),
            Csv(q.LegalEntitlementL2L3_FundingApprovalEndDate),
            Csv("Missing from view" /*q.LegalEntitlementL2L3_Notes*/),

            Csv(q.LegalEntitlementEnglishandMaths_FundingAvailable?.ToString()),
            Csv(q.LegalEntitlementEnglishandMaths_FundingApprovalStartDate),
            Csv(q.LegalEntitlementEnglishandMaths_FundingApprovalEndDate),
            Csv("Missing from view" /*q.LegalEntitlementEnglishandMaths_Notes*/),

            Csv(q.DigitalEntitlement_FundingAvailable?.ToString()),
            Csv(q.DigitalEntitlement_FundingApprovalStartDate),
            Csv(q.DigitalEntitlement_FundingApprovalEndDate),
            Csv("Missing from view" /*q.DigitalEntitlement_Notes*/),

            Csv(q.LifelongLearningEntitlement_FundingAvailable?.ToString()),
            Csv(q.LifelongLearningEntitlement_FundingApprovalStartDate),
            Csv(q.LifelongLearningEntitlement_FundingApprovalEndDate),
            Csv("Missing from view" /*q.LifelongLearningEntitlement_Notes*/),

            Csv(q.AdvancedLearnerLoans_FundingAvailable?.ToString()),
            Csv(q.AdvancedLearnerLoans_FundingApprovalStartDate),
            Csv(q.AdvancedLearnerLoans_FundingApprovalEndDate),
            Csv("Missing from view" /*q.AdvancedLearnerLoans_Notes*/),

            Csv("Missing from view" /*q.AwardingOrganisationURL*/),

            Csv(q.FreeCoursesForJobs_FundingAvailable?.ToString()),
            Csv(q.FreeCoursesForJobs_FundingApprovalStartDate),
            Csv(q.FreeCoursesForJobs_FundingApprovalEndDate),
            Csv("Missing from view" /*q.FreeCoursesForJobs_Notes*/),
        });

            await w.WriteLineAsync(line);
        }

        await w.FlushAsync();
    }

}


