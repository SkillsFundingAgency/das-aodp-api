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
                if (request.File == null || request.File.Length == 0)
                {
                    response.Success = true;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                var items = new List<PLDNS>();

                await using var ms = new MemoryStream();
                await request.File.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;

                using var document = SpreadsheetDocument.Open(ms, false);
                var workbookPart = document.WorkbookPart ?? throw new InvalidOperationException("Workbook part missing.");
                var sharedStrings = workbookPart.SharedStringTablePart?.SharedStringTable;

                var targetSheetName = "PLDNS V12F";
                Sheet? chosenSheet = workbookPart.Workbook.Sheets!
                    .Cast<Sheet?>()
                    .FirstOrDefault(s => string.Equals((s?.Name!.Value ?? string.Empty).Trim(), targetSheetName, StringComparison.OrdinalIgnoreCase));

                if (chosenSheet == null)
                {
                    response.Success = true;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(chosenSheet.Id!);
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

                Row headerRow = rows.Count > 1 ? rows[1] : rows[0];
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
                        headerRow = rows[r];
                        headerListIndex = r;
                        break;
                    }
                }

                var headerMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in headerRow.Elements<Cell>())
                {
                    var col = GetColumnName(cell.CellReference?.Value);
                    var txt = GetCellText(cell, sharedStrings);
                    if (!string.IsNullOrWhiteSpace(col) && !string.IsNullOrWhiteSpace(txt))
                        headerMap[col!] = txt.Trim();
                }

                string? qanCol = FindColumn(headerMap, "text QAN", "QAN", "Qualification number", "Qualification");
                if (qanCol == null)
                {
                    qanCol = headerMap.FirstOrDefault(kv => (kv.Value ?? string.Empty).ToLowerInvariant().Contains("qan")).Key;
                }

                string? listUpdatedCol = FindColumn(headerMap, "Date PLDNS list updated", "Date PLDNS list updated", "list updated");
                string? noteCol = FindColumn(headerMap, "NOTE", "Notes", "NOTE: OED at rollover before 01/08/2019. not for 19/20 or 20/21 offer");

                string? p14to16Col = FindColumn(headerMap, "PLDNS 14-16");
                string? p14to16NotesCol = FindColumn(headerMap, "NOTES PLDNS 14-16", "NOTES PLDNS 14-16 :");

                string? p16to19Col = FindColumn(headerMap, "PLDNS 16-19");
                string? p16to19NotesCol = FindColumn(headerMap, "NOTES PLDNS 16-19", "NOTES PLDNS 16-19:");

                string? localFlexCol = FindColumn(headerMap, "PLDNS Local flex");
                string? localFlexNotesCol = FindColumn(headerMap, "NOTES PLDNS Local flex");

                string? legalL2L3Col = FindColumn(headerMap, "PLDNS Legal entitlement L2/L3");
                string? legalL2L3NotesCol = FindColumn(headerMap, "NOTES PLDNS Legal entitlement L2/L3");

                string? legalEngMathsCol = FindColumn(headerMap, "PLDNS Legal entitlement Eng/Maths");
                string? legalEngMathsNotesCol = FindColumn(headerMap, "NOTES PLDNS Legal entitlement Eng/Maths");

                string? digitalCol = FindColumn(headerMap, "PLDNS Digital entitlement");
                string? digitalNotesCol = FindColumn(headerMap, "NOTES PLDNS Digital entitlement");

                string? esfCol = FindColumn(headerMap, "PLDNS ESF L3/L4");
                string? esfNotesCol = FindColumn(headerMap, "NOTES PLDNS ESF L3/L4");

                string? loansCol = FindColumn(headerMap, "PLDNS Loans");
                string? loansNotesCol = FindColumn(headerMap, "NOTES PLDNS Loans");

                string? lleCol = FindColumn(headerMap, "PLDNS Lifelong Learning Entitlement");
                string? lleNotesCol = FindColumn(headerMap, "NOTES PLDNS Lifelong Learning Entitlement");

                string? fcfjCol = FindColumn(headerMap, "PLDNS Level 3 Free Courses for Jobs");
                string? fcfjNotesCol = FindColumn(headerMap, "Level 3 Free Courses for Jobs (Previously known as National skills fund L3 extended entitlement)");

                string? cofCol = FindColumn(headerMap, "PLDNS CoF");
                string? cofNotesCol = FindColumn(headerMap, "NOTES  PLDNS CoF");

                string? startDateCol = FindColumn(headerMap, "Start date");
                string? startDateNotesCol = FindColumn(headerMap, "NOTES Start date");

                var culture = new CultureInfo("en-GB");
                var dateFormats = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd MMM yyyy" };


                var dataStartIndex = headerListIndex + 1;

                for (int i = dataStartIndex; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var rowIndex = row.RowIndex?.Value.ToString() ?? (i + 1).ToString();

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

                    if (string.IsNullOrWhiteSpace(qanCol))
                    {
                        break;
                    }

                    if (!cellMap.TryGetValue(qanCol, out var qNumber) || string.IsNullOrWhiteSpace(qNumber))
                        continue;

                    var entity = new PLDNS
                    {
                        Qan = qNumber!.Trim(),

                        ListUpdatedDate = TryParseDate(GetValue(cellMap, listUpdatedCol), culture, dateFormats),
                        Notes = GetValue(cellMap, noteCol)?.Trim(),

                        Pldns14To16 = TryParseDate(GetValue(cellMap, p14to16Col), culture, dateFormats),
                        Pldns14To16Note = GetValue(cellMap, p14to16NotesCol)?.Trim(),

                        Pldns16To19 = TryParseDate(GetValue(cellMap, p16to19Col), culture, dateFormats),
                        Pldns16To19Note = GetValue(cellMap, p16to19NotesCol)?.Trim(),

                        LocalFlex = TryParseDate(GetValue(cellMap, localFlexCol), culture, dateFormats),
                        LocalFlexNote = GetValue(cellMap, localFlexNotesCol)?.Trim(),

                        LegalEntitlementL2L3 = TryParseDate(GetValue(cellMap, legalL2L3Col), culture, dateFormats),
                        LegalEntitlementL2L3Note = GetValue(cellMap, legalL2L3NotesCol)?.Trim(),

                        LegalEntitlementEngMaths = TryParseDate(GetValue(cellMap, legalEngMathsCol), culture, dateFormats),
                        LegalEntitlementEngMathsNote = GetValue(cellMap, legalEngMathsNotesCol)?.Trim(),

                        DigitalEntitlement = TryParseDate(GetValue(cellMap, digitalCol), culture, dateFormats),
                        DigitalEntitlementNote = GetValue(cellMap, digitalNotesCol)?.Trim(),

                        EsfL3L4 = TryParseDate(GetValue(cellMap, esfCol), culture, dateFormats),
                        EsfL3L4Note = GetValue(cellMap, esfNotesCol)?.Trim(),

                        Loans = TryParseDate(GetValue(cellMap, loansCol), culture, dateFormats),
                        LoansNote = GetValue(cellMap, loansNotesCol)?.Trim(),

                        LifelongLearning = TryParseDate(GetValue(cellMap, lleCol), culture, dateFormats),
                        LifelongLearningNote = GetValue(cellMap, lleNotesCol)?.Trim(),

                        Level3FCoursesForJobs = TryParseDate(GetValue(cellMap, fcfjCol), culture, dateFormats),
                        Level3FCoursesForJobsNote = GetValue(cellMap, fcfjNotesCol)?.Trim(),

                        Cof = TryParseDate(GetValue(cellMap, cofCol), culture, dateFormats),
                        CofNote = GetValue(cellMap, cofNotesCol)?.Trim(),

                        StartDate = TryParseDate(GetValue(cellMap, startDateCol), culture, dateFormats),
                        StartDateNote = GetValue(cellMap, startDateNotesCol)?.Trim(),

                        ImportDate = DateTime.UtcNow
                    };

                    items.Add(entity);
                }

                if (!items.Any())
                {
                    response.Success = true;
                    response.Value = new ImportPLDNSCommandResponse { ImportedCount = 0 };
                    return response;
                }

                var totalImported = 0;
                var batches = (int)Math.Ceiling(items.Count / (double)BatchSize);
                for (var batch = 0; batch < batches; batch++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var batchItems = items.Skip(batch * BatchSize).Take(BatchSize).ToList();
                    try
                    {
                        await _repository.BulkInsertAsync(batchItems, cancellationToken);
                        totalImported += batchItems.Count;
                    }
                    catch (Exception batchEx)
                    {
                        response.Success = false;
                        response.ErrorMessage = $"Failed inserting batch {batch + 1} of {batches}: {batchEx.Message}";
                        response.InnerException = batchEx;
                        return response;
                    }
                }

                var deletedRows = await _repository.DeleteDuplicatePLDNSAsync(null, cancellationToken);

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