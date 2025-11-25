using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Import;
using System.Globalization;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Import
{
    public class ImportPLDNSCommandHandler : IRequestHandler<ImportPLDNSCommand, BaseMediatrResponse<ImportPLDNSCommandResponse>>
    {
        private readonly IPLDNSRepository _repository;
        private const int BatchSize = 3000;

        public ImportPLDNSCommandHandler(IPLDNSRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<ImportPLDNSCommandResponse>> Handle(ImportPLDNSCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<ImportPLDNSCommandResponse>();

            try
            {
                // Validate request and file type
                var (IsValid, Success, ErrorMessage) = ValidateRequest(request);
                if (!IsValid)
                {
                    response.Success = Success;
                    response.ErrorMessage = ErrorMessage;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                await using var ms = new MemoryStream();
                await request.File!.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;

                using var document = SpreadsheetDocument.Open(ms, false);
                var workbookPart = document.WorkbookPart ?? throw new InvalidOperationException("Workbook part missing.");
                var sharedStrings = workbookPart.SharedStringTablePart?.SharedStringTable;

                var sheet = FindSheet(workbookPart, "PLDNS V12F");
                if (sheet == null)
                {
                    response.Success = true;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                if (sheetData == null)
                {
                    response.Success = true;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                var rows = sheetData.Elements<Row>().ToList();
                if (rows.Count <= 1)
                {
                    response.Success = true;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                int headerIndex = FindHeaderIndex(rows, sharedStrings);
                var headerRow = rows[headerIndex];

                var headerMap = BuildHeaderMap(headerRow, sharedStrings);
                var columns = MapColumns(headerMap);

                var culture = new CultureInfo("en-GB");
                var dateFormats = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd MMM yyyy" };

                var items = ParseRowsToEntities(rows, headerIndex + 1, sharedStrings, headerMap, columns, culture, dateFormats);
                if (!items.Any())
                {
                    response.Success = true;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                var totalImported = await InsertBatchesAsync(items, cancellationToken);

                await _repository.DeleteDuplicatePLDNSAsync(null, cancellationToken);

                response.Success = true;
                response.Value = new ImportPLDNSCommandResponse { ImportedCount = totalImported };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
            }

            return response;
        }

        private static (bool IsValid, bool Success, string? ErrorMessage) ValidateRequest(ImportPLDNSCommand request)
        {
            if (request.File == null || request.File.Length == 0)
                return (false, true, null);

            if (string.IsNullOrWhiteSpace(request.FileName) || !request.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                return (false, false, "Unsupported file type. Only .xlsx files are accepted.");

            return (true, false, null);
        }

        private static Sheet? FindSheet(WorkbookPart workbookPart, string targetSheetName)
        {
            return workbookPart.Workbook.Sheets!
                .Cast<Sheet?>()
                .FirstOrDefault(s => string.Equals((s?.Name!.Value ?? string.Empty).Trim(), targetSheetName, StringComparison.OrdinalIgnoreCase));
        }

        private static int FindHeaderIndex(List<Row> rows, SharedStringTable? sharedStrings)
        {
            var headerRow = rows.Count > 1 ? rows[1] : rows[0];
            int headerListIndex = rows.IndexOf(headerRow);

            var headerKeywords = new[] {
                "text qan","list updated","note",
                "pldns 14-16","pldns 16-19","pldns local flex",
                "legal entitlement","digital entitlement","esf l3/l4",
                "pldns loans","lifelong learning entitlement","level 3 free courses",
                "pldns cof","start date"
            };

            for (int r = 0; r < Math.Min(rows.Count, 12); r++)
            {
                var cellTexts = rows[r].Elements<Cell>()
                    .Select(c => GetCellText(c, sharedStrings).Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.ToLowerInvariant())
                    .ToList();

                if (cellTexts.Count == 0) continue;

                var matches = cellTexts.Count(ct => headerKeywords.Any(k => ct.Contains(k)));
                if (matches >= 1)
                {
                    headerListIndex = r;
                    break;
                }
            }

            return headerListIndex;
        }

        private static Dictionary<string, string> BuildHeaderMap(Row headerRow, SharedStringTable? sharedStrings)
        {
            var headerMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in headerRow.Elements<Cell>())
            {
                var col = GetColumnName(cell.CellReference?.Value);
                var txt = GetCellText(cell, sharedStrings);
                if (!string.IsNullOrWhiteSpace(col) && !string.IsNullOrWhiteSpace(txt))
                    headerMap[col!] = txt.Trim();
            }
            return headerMap;
        }

        private sealed record ColumnNames(
            string? Qan,
            string? ListUpdated,
            string? Note,
            string? P14To16,
            string? P14To16Note,
            string? P16To19,
            string? P16To19Note,
            string? LocalFlex,
            string? LocalFlexNote,
            string? LegalL2L3,
            string? LegalL2L3Note,
            string? LegalEngMaths,
            string? LegalEngMathsNote,
            string? Digital,
            string? DigitalNote,
            string? Esf,
            string? EsfNote,
            string? Loans,
            string? LoansNote,
            string? Lle,
            string? LleNote,
            string? Fcfj,
            string? FcfjNote,
            string? Cof,
            string? CofNote,
            string? StartDate,
            string? StartDateNote
        );

        private static ColumnNames MapColumns(IDictionary<string, string> headerMap)
        {
            string? Find(params string[] variants) => FindColumn(headerMap, variants);

            var qanCol = Find("text QAN", "QAN", "Qualification number", "Qualification")
                          ?? headerMap.FirstOrDefault(kv => (kv.Value ?? string.Empty).ToLowerInvariant().Contains("qan")).Key;

            return new ColumnNames(
                qanCol,
                Find("Date PLDNS list updated", "Date PLDNS list updated", "list updated"),
                Find("NOTE", "Notes", "NOTE: OED at rollover before 01/08/2019. not for 19/20 or 20/21 offer"),
                Find("PLDNS 14-16"),
                Find("NOTES PLDNS 14-16", "NOTES PLDNS 14-16 :"),
                Find("PLDNS 16-19"),
                Find("NOTES PLDNS 16-19", "NOTES PLDNS 16-19:"),
                Find("PLDNS Local flex"),
                Find("NOTES PLDNS Local flex"),
                Find("PLDNS Legal entitlement L2/L3"),
                Find("NOTES PLDNS Legal entitlement L2/L3"),
                Find("PLDNS Legal entitlement Eng/Maths"),
                Find("NOTES PLDNS Legal entitlement Eng/Maths"),
                Find("PLDNS Digital entitlement"),
                Find("NOTES PLDNS Digital entitlement"),
                Find("PLDNS ESF L3/L4"),
                Find("NOTES PLDNS ESF L3/L4"),
                Find("PLDNS Loans"),
                Find("NOTES PLDNS Loans"),
                Find("PLDNS Lifelong Learning Entitlement"),
                Find("NOTES PLDNS Lifelong Learning Entitlement"),
                Find("PLDNS Level 3 Free Courses for Jobs"),
                Find("Level 3 Free Courses for Jobs (Previously known as National skills fund L3 extended entitlement)"),
                Find("PLDNS CoF"),
                Find("NOTES  PLDNS CoF"),
                Find("Start date"),
                Find("NOTES Start date")
            );
        }

        private static List<PLDNS> ParseRowsToEntities(
            List<Row> rows,
            int startIndex,
            SharedStringTable? sharedStrings,
            IDictionary<string, string> headerMap,
            ColumnNames columns,
            CultureInfo culture,
            string[] dateFormats)
        {
            var items = new List<PLDNS>();
            for (int i = startIndex; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowCells = row.Elements<Cell>();
                if (rowCells == null) continue;

                var cellMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in rowCells)
                {
                    var col = GetColumnName(cell.CellReference?.Value);
                    if (string.IsNullOrWhiteSpace(col)) continue;
                    var text = GetCellText(cell, sharedStrings)?.Trim() ?? string.Empty;
                    if (!cellMap.ContainsKey(col)) cellMap[col] = text;
                }

                if (string.IsNullOrWhiteSpace(columns.Qan))
                    break;

                if (!cellMap.TryGetValue(columns.Qan, out var qNumber) || string.IsNullOrWhiteSpace(qNumber))
                    continue;

                PLDNS CreateEntity() => new PLDNS
                {
                    Qan = qNumber!.Trim(),

                    ListUpdatedDate = TryParseDate(GetValue(cellMap, columns.ListUpdated), culture, dateFormats),
                    Notes = GetValue(cellMap, columns.Note)?.Trim(),

                    Pldns14To16 = TryParseDate(GetValue(cellMap, columns.P14To16), culture, dateFormats),
                    Pldns14To16Note = GetValue(cellMap, columns.P14To16Note)?.Trim(),

                    Pldns16To19 = TryParseDate(GetValue(cellMap, columns.P16To19), culture, dateFormats),
                    Pldns16To19Note = GetValue(cellMap, columns.P16To19Note)?.Trim(),

                    LocalFlex = TryParseDate(GetValue(cellMap, columns.LocalFlex), culture, dateFormats),
                    LocalFlexNote = GetValue(cellMap, columns.LocalFlexNote)?.Trim(),

                    LegalEntitlementL2L3 = TryParseDate(GetValue(cellMap, columns.LegalL2L3), culture, dateFormats),
                    LegalEntitlementL2L3Note = GetValue(cellMap, columns.LegalL2L3Note)?.Trim(),

                    LegalEntitlementEngMaths = TryParseDate(GetValue(cellMap, columns.LegalEngMaths), culture, dateFormats),
                    LegalEntitlementEngMathsNote = GetValue(cellMap, columns.LegalEngMathsNote)?.Trim(),

                    DigitalEntitlement = TryParseDate(GetValue(cellMap, columns.Digital), culture, dateFormats),
                    DigitalEntitlementNote = GetValue(cellMap, columns.DigitalNote)?.Trim(),

                    EsfL3L4 = TryParseDate(GetValue(cellMap, columns.Esf), culture, dateFormats),
                    EsfL3L4Note = GetValue(cellMap, columns.EsfNote)?.Trim(),

                    Loans = TryParseDate(GetValue(cellMap, columns.Loans), culture, dateFormats),
                    LoansNote = GetValue(cellMap, columns.LoansNote)?.Trim(),

                    LifelongLearning = TryParseDate(GetValue(cellMap, columns.Lle), culture, dateFormats),
                    LifelongLearningNote = GetValue(cellMap, columns.LleNote)?.Trim(),

                    Level3FCoursesForJobs = TryParseDate(GetValue(cellMap, columns.Fcfj), culture, dateFormats),
                    Level3FCoursesForJobsNote = GetValue(cellMap, columns.FcfjNote)?.Trim(),

                    Cof = TryParseDate(GetValue(cellMap, columns.Cof), culture, dateFormats),
                    CofNote = GetValue(cellMap, columns.CofNote)?.Trim(),

                    StartDate = TryParseDate(GetValue(cellMap, columns.StartDate), culture, dateFormats),
                    StartDateNote = GetValue(cellMap, columns.StartDateNote)?.Trim(),

                    ImportDate = DateTime.UtcNow
                };

                items.Add(CreateEntity());
            }

            return items;
        }

        private async Task<int> InsertBatchesAsync(List<PLDNS> items, CancellationToken cancellationToken)
        {
            var totalImported = 0;
            var batches = (int)Math.Ceiling(items.Count / (double)BatchSize);
            for (var batch = 0; batch < batches; batch++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batchItems = items.Skip(batch * BatchSize).Take(BatchSize).ToList();
                await _repository.BulkInsertAsync(batchItems, cancellationToken);
                totalImported += batchItems.Count;
            }
            return totalImported;
        }

        private static string? GetValue(IDictionary<string, string> map, string? column) =>
                        string.IsNullOrWhiteSpace(column) ? null : (map.TryGetValue(column!, out var v) ? v : null);

        private static string? FindColumn(IDictionary<string, string> headerMap, params string[] variants)
        {
            if (headerMap == null || variants == null || variants.Length == 0) return null;

            // exact match first
            foreach (var kv in headerMap)
            {
                var header = kv.Value?.Trim();
                if (string.IsNullOrEmpty(header)) continue;

                if (variants.Any(v => string.Equals(v.Trim(), header, StringComparison.OrdinalIgnoreCase)))
                    return kv.Key;
            }

            // contains match
            foreach (var kv in headerMap)
            {
                var header = kv.Value?.Trim().ToLowerInvariant() ?? string.Empty;
                foreach (var v in variants)
                {
                    var variant = v?.Trim().ToLowerInvariant() ?? string.Empty;
                    if (!string.IsNullOrEmpty(variant) && header.Contains(variant))
                        return kv.Key;
                }
            }

            return null;
        }

        private static string GetCellText(Cell cell, SharedStringTable? sharedStrings)
        {
            if (cell == null) return string.Empty;

            if (cell.DataType != null && cell.DataType.Value == CellValues.InlineString)
                return cell.InnerText ?? string.Empty;

            var value = cell.CellValue?.InnerText ?? string.Empty;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                if (int.TryParse(value, out var sstIndex) && sharedStrings != null)
                {
                    var ssi = sharedStrings.Elements().ElementAtOrDefault(sstIndex);
                    if (ssi == null) return string.Empty;
                    var text = ssi.InnerText?.ToString();
                    if (!string.IsNullOrEmpty(text)) return text;
                    var runText = ssi.Elements<Run>().SelectMany(r => r.Elements<Text>()).Select(t => t.Text).FirstOrDefault();
                    if (!string.IsNullOrEmpty(runText)) return runText;
                    return ssi.InnerText ?? string.Empty;
                }
            }

            if (cell.DataType != null && cell.DataType.Value == CellValues.Boolean)
                return value == "1" ? "TRUE" : value == "0" ? "FALSE" : value;

            return value;
        }

        private static string? GetColumnName(string? cellReference)
        {
            if (string.IsNullOrWhiteSpace(cellReference)) return null;
            var sb = new StringBuilder();
            foreach (var ch in cellReference)
            {
                if (char.IsLetter(ch)) sb.Append(ch);
                else break;
            }
            return sb.ToString();
        }

        private static DateTime? TryParseDate(string? txt, CultureInfo culture, string[] formats)
        {
            if (string.IsNullOrWhiteSpace(txt)) return null;
            txt = txt!.Trim();
            if (DateTime.TryParse(txt, culture, DateTimeStyles.None, out var dt)) return dt.Date;
            if (DateTime.TryParseExact(txt, formats, culture, DateTimeStyles.None, out dt)) return dt.Date;
            if (double.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var oa))
            {
                try { return DateTime.FromOADate(oa); } catch { }
            }
            return null;
        }
    }
}