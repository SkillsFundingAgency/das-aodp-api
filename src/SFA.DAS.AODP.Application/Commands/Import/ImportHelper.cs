using DocumentFormat.OpenXml.Spreadsheet;

namespace SFA.DAS.AODP.Application.Commands.Import;

public static class ImportHelper
{
    public static string GetCellText(Cell cell, SharedStringTable? sharedStrings)
    {
        if (cell == null)
        {
            return string.Empty;
        }

        if (cell.DataType != null && cell.DataType.Value == CellValues.InlineString)
        {
            return cell.InnerText ?? string.Empty;
        }

        var value = cell.CellValue?.InnerText ?? string.Empty;

        if (cell.DataType == null)
        {
            return value;
        }

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

        return value;
    }

    public static string? FindColumn(IDictionary<string, string> headerMap, params string[] variants)
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
}
