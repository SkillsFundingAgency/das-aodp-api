using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Import;
using System.Data;
using System.Text;

namespace SFA.DAS.AODP.Application.Commands.Import;

public class ImportDefundingListCommandHandler : IRequestHandler<ImportDefundingListCommand, BaseMediatrResponse<ImportDefundingListCommandResponse>>
{
    private readonly IDefundingListRepository _repository;

    public ImportDefundingListCommandHandler(IDefundingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseMediatrResponse<ImportDefundingListCommandResponse>> Handle(ImportDefundingListCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<ImportDefundingListCommandResponse>();

        try
        {
            // Validate request and file type
            var (IsValid, Success, ErrorMessage) = ValidateRequest(request);
            if (!IsValid)
            {
                response.Success = Success;
                response.ErrorMessage = ErrorMessage;
                response.Value = new ImportDefundingListCommandResponse { ImportedCount = 0 };
                return response;
            }

            // Load file into memory
            await using var ms = new MemoryStream();
            await request.File!.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;

            using var document = SpreadsheetDocument.Open(ms, false);
            var workbookPart = document.WorkbookPart ?? throw new InvalidOperationException("Workbook part missing.");
            var sharedStrings = workbookPart.SharedStringTablePart?.SharedStringTable;

            // Get target sheet
            var targetSheetName = "Approval not extended";
            var chosenSheet = workbookPart.Workbook.Sheets!
                .Cast<Sheet?>()
                .FirstOrDefault(s => string.Equals((s?.Name!.Value ?? string.Empty).Trim(), targetSheetName, StringComparison.OrdinalIgnoreCase));

            if (chosenSheet == null)
            {
                response.Success = true;
                response.Value = new ImportDefundingListCommandResponse { ImportedCount = 0 };
                return response;
            }

            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(chosenSheet.Id!);
            var rows = GetRowsFromWorksheet(worksheetPart).ToList();
            if (rows.Count <= 1)
            {
                response.Success = true;
                response.Value = new ImportDefundingListCommandResponse { ImportedCount = 0 };
                return response;
            }

            // Detect header row
            var (headerRow, headerIndex) = DetectHeaderRow(rows, sharedStrings);

            // Build header map
            var headerMap = BuildHeaderMap(headerRow, sharedStrings);

            // Parse data rows into items
            var items = ParseDataRows(rows, headerIndex + 1, headerMap, worksheetPart, sharedStrings);

            if (items.Count == 0)
            {
                response.Success = true;
                response.Value = new ImportDefundingListCommandResponse { ImportedCount = 0 };
                return response;
            }

            await _repository.BulkInsertAsync(items, cancellationToken);
            await _repository.DeleteDuplicateDefundingListsAsync(null, cancellationToken);

            response.Success = true;
            response.Value = new ImportDefundingListCommandResponse { ImportedCount = items.Count };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }

        return response;
    }

    private static (bool IsValid, bool Success, string? ErrorMessage) ValidateRequest(ImportDefundingListCommand request)
    {
        if (request.File == null || request.File.Length == 0)
            return (false, true, null);

        if (string.IsNullOrWhiteSpace(request.FileName) || !request.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return (false, false, "Unsupported file type. Only .xlsx files are accepted.");

        return (true, false, null);
    }

    private static IEnumerable<Row> GetRowsFromWorksheet(WorksheetPart worksheetPart)
    {
        var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
        if (sheetData == null) yield break;
        foreach (var row in sheetData.Elements<Row>()) yield return row;
    }

    private static (Row headerRow, int headerIndex) DetectHeaderRow(List<Row> rows, SharedStringTable? sharedStrings)
    {
        Row headerRow = rows.Count > 6 ? rows[6] : rows[0];
        int headerListIndex = rows.IndexOf(headerRow);

        var headerKeywords = new[] { "qualification", "qan", "title", "award", "guided", "sector", "route", "funding", "in scope", "comments" };

        for (int r = 0; r < Math.Min(rows.Count, 12); r++)
        {
            var cellTexts = rows[r].Elements<Cell>()
                .Select(c => GetCellText(c, sharedStrings).Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.ToLowerInvariant())
                .ToList();

            if (cellTexts.Count == 0) continue;

            var matches = cellTexts.Count(ct => headerKeywords.Any(k => ct.Contains(k)));
            if (matches >= 2)
            {
                headerRow = rows[r];
                headerListIndex = r;
                break;
            }
        }

        return (headerRow, headerListIndex);
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

    private static List<DefundingList> ParseDataRows(List<Row> rows, int startIndex, IDictionary<string, string> headerMap, WorksheetPart worksheetPart, SharedStringTable? sharedStrings)
    {
        var items = new List<DefundingList>();
        if (startIndex < 0) startIndex = 0;

        string? qCol = FindColumn(headerMap, "Qualification number");
        string? titleCol = FindColumn(headerMap, "Title");
        string? awardingCol = FindColumn(headerMap, "Awarding organisation");
        string? glhCol = FindColumn(headerMap, "Guided Learning Hours");
        string? ssaCol = FindColumn(headerMap, "Sector Subject Area Tier 2");
        string? routeCol = FindColumn(headerMap, "Relevant route");
        string? fundingCol = FindColumn(headerMap, "Funding offer");
        string? inScopeCol = FindColumn(headerMap, "InScope", "In Scope");
        string? commentsCol = FindColumn(headerMap, "Comments");

        for (int i = startIndex; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowIndex = row.RowIndex?.Value.ToString() ?? (i + 1).ToString();

            var qNumber = GetCellTextByColumn(worksheetPart, rowIndex, qCol, sharedStrings);
            if (string.IsNullOrWhiteSpace(qNumber))
                continue;

            var title = GetCellTextByColumn(worksheetPart, rowIndex, titleCol, sharedStrings);
            var awardingOrg = GetCellTextByColumn(worksheetPart, rowIndex, awardingCol, sharedStrings);
            var glh = GetCellTextByColumn(worksheetPart, rowIndex, glhCol, sharedStrings);
            var ssa = GetCellTextByColumn(worksheetPart, rowIndex, ssaCol, sharedStrings);
            var route = GetCellTextByColumn(worksheetPart, rowIndex, routeCol, sharedStrings);
            var fundingOffer = GetCellTextByColumn(worksheetPart, rowIndex, fundingCol, sharedStrings);
            var inScopeStr = GetCellTextByColumn(worksheetPart, rowIndex, inScopeCol, sharedStrings);
            var comments = GetCellTextByColumn(worksheetPart, rowIndex, commentsCol, sharedStrings);

            bool inScope = ParseInScope(inScopeStr);

            items.Add(new DefundingList
            {
                Qan = qNumber,
                Title = string.IsNullOrWhiteSpace(title) ? null : title,
                AwardingOrganisation = string.IsNullOrWhiteSpace(awardingOrg) ? null : awardingOrg,
                GuidedLearningHours = string.IsNullOrWhiteSpace(glh) ? null : glh,
                SectorSubjectArea = string.IsNullOrWhiteSpace(ssa) ? null : ssa,
                RelevantRoute = string.IsNullOrWhiteSpace(route) ? null : route,
                FundingOffer = string.IsNullOrWhiteSpace(fundingOffer) ? null : fundingOffer,
                InScope = inScope,
                Comments = string.IsNullOrWhiteSpace(comments) ? null : comments,
                ImportDate = DateTime.UtcNow
            });
        }

        return items;
    }

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
        {
            return cell.InnerText ?? string.Empty;
        }

        var value = cell.CellValue?.InnerText ?? string.Empty;

        if (cell.DataType != null)
        {
            if (cell.DataType.Value == CellValues.SharedString)
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
            else if (cell.DataType.Value == CellValues.Boolean)
            {
                return value switch
                {
                    "1" => "TRUE",
                    "0" => "FALSE",
                    _ => value
                };
            }
            else
            {
                return value;
            }
        }

        return value;
    }

    private static bool ParseInScope(string? inScopeStr)
    {
        if (string.IsNullOrWhiteSpace(inScopeStr)) return true;
        var normalized = inScopeStr.Trim().ToLowerInvariant();
        if (normalized is "0" or "false" or "no" or "n" or "excluded") return false;
        if (normalized is "1" or "true" or "yes" or "y" or "included") return true;
        if (bool.TryParse(inScopeStr, out var b)) return b;
        if (int.TryParse(inScopeStr, out var i)) return i != 0;
        return true;
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

    private static string GetCellTextByColumn(WorksheetPart worksheetPart, string rowIndex, string? column, SharedStringTable? sharedStrings)
    {
        if (string.IsNullOrWhiteSpace(column) || string.IsNullOrWhiteSpace(rowIndex)) return string.Empty;
        var address = $"{column}{rowIndex}";
        var cell = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => string.Equals((c.CellReference ?? "").Value, address, StringComparison.OrdinalIgnoreCase));
        if (cell == null) return string.Empty;
        return GetCellText(cell, sharedStrings)?.Trim() ?? string.Empty;
    }
}
