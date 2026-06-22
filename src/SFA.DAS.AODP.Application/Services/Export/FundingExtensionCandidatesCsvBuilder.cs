using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Models.Rollover;
using System.Text;

namespace SFA.DAS.AODP.Application.Services.Export
{
    public interface IFundingExtensionCandidatesCsvBuilder
    {
        // Builds a CSV containing the core candidate details for review .
        byte[] Build(IEnumerable<RolloverCandidateForExport> rows);

        // Builds a CSV containing core candidate details plus a column for 'ValidationErrors'.
        byte[] BuildWithValidationErrors(
            IEnumerable<RolloverCandidateForExport> rows,
            IEnumerable<CandidateValidationResult> validationResults);
    }

    public class FundingExtensionCandidatesCsvBuilder : IFundingExtensionCandidatesCsvBuilder
    {
        private static readonly string[] CoreColumns =
            [
                "QAN",
                "QualificationTitle",
                "AwardingOrganisation",
                "QualificationLevel",
                "QualificationType",
                "SSA",
                "OperationalEndDate",
                "OfferedInEngland",
                "FundedInEngland",
                "GLH",
                "TQT",
                "Pre16",
                "Age16To18",
                "Age18Plus",
                "Age19Plus",
                "FundingStreamName",
                "FundingApprovalStartDate",
                "ProposedOutcome",
                "RolloverStatus",
                "ExclusionReason",
                "CurrentFundingApprovalEndDate",
                "ProposedFundingApprovalEndDate",
                "Comments"
            ];
        private static readonly string[] ColumnsWithErrors = CoreColumns.Append("ValidationErrors").ToArray();

        public byte[] Build(IEnumerable<RolloverCandidateForExport> rows)
        {
            using var ms = new MemoryStream(4096);

            using (var writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true))
            {
                writer.WriteLine(string.Join(",", CoreColumns));

                foreach (var r in rows)
                {
                    WriteCoreColumns(writer, r);
                    writer.WriteLine(); 
                }
            }

            return ms.ToArray();
        }

        public byte[] BuildWithValidationErrors(
            IEnumerable<RolloverCandidateForExport> rows,
            IEnumerable<CandidateValidationResult> validationResults)
        {
            using var ms = new MemoryStream(4096);

            using (var writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true))
            {
                // Write the 23 core headers + ValidationErrors
                WriteHeader(writer, ColumnsWithErrors);

                foreach (var r in rows)
                {
                    WriteCoreColumns(writer, r);
                    writer.Write(',');

                    var result = validationResults
                        .FirstOrDefault(v => v.CandidateDetails.RowNumber == r.RowNumber);

                    var errorText = result?.Errors?.Count > 0
                        ? string.Join("; ", result.Errors.Select(e => e.Message))
                        : string.Empty;

                    WriteField(writer, errorText);

                    writer.WriteLine();
                }
            }

            return ms.ToArray();
        }


        private static void WriteHeader(StreamWriter writer, IEnumerable<string> columns)
        {
            writer.WriteLine(string.Join(",", columns));
        }
        private static void WriteCoreColumns(StreamWriter writer, RolloverCandidateForExport r)
        {
            WriteField(writer, r.QAN); writer.Write(',');
            WriteField(writer, r.QualificationTitle); writer.Write(',');
            WriteField(writer, r.AwardingOrganisation); writer.Write(',');
            WriteField(writer, r.QualificationLevel); writer.Write(',');
            WriteField(writer, r.QualificationType); writer.Write(',');
            WriteField(writer, r.SSA); writer.Write(',');
            WriteField(writer, r.OperationalEndDate); writer.Write(',');
            WriteField(writer, r.OfferedInEngland); writer.Write(',');
            WriteField(writer, r.FundedInEngland); writer.Write(',');
            WriteField(writer, r.GLH); writer.Write(',');
            WriteField(writer, r.TQT); writer.Write(',');
            WriteField(writer, r.Pre16); writer.Write(',');
            WriteField(writer, r.Age16To18); writer.Write(',');
            WriteField(writer, r.Age18Plus); writer.Write(',');
            WriteField(writer, r.Age19Plus); writer.Write(',');
            WriteField(writer, r.FundingStreamName); writer.Write(',');
            WriteField(writer, r.FundingApprovalStartDate); writer.Write(',');
            WriteField(writer, r.ProposedOutcome); writer.Write(',');
            WriteField(writer, r.RolloverStatus); writer.Write(',');
            WriteField(writer, r.ExclusionReason); writer.Write(',');
            WriteField(writer, r.CurrentFundingApprovalEndDate); writer.Write(',');
            WriteField(writer, r.ProposedFundingApprovalEndDate); writer.Write(',');
            WriteField(writer, r.Comments);
        }

        /// <summary>
        /// Optimized CSV writer that prints content straight to the output stream.
        /// Eliminates intermediate object creation and string allocations for safe/standard types.
        /// </summary>
        private static void WriteField(StreamWriter writer, object? value)
        {
            if (value == null) return;

            if (value is DateTime dt)
            {
                writer.Write(dt.ToCsvDateFormat());
                return;
            }

            if (value is DateOnly doVal)
            {
                writer.Write(doVal.ToCsvDateFormat());
                return;
            }
            if (value is bool b)
            {
                writer.Write(b ? "True" : "False");
                return;
            }

            var s = value.ToString() ?? "";

            var needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            if (needsQuotes)
            {
                writer.Write('"');
                writer.Write(s.Replace("\"", "\"\""));
                writer.Write('"');
            }
            else
            {
                writer.Write(s);
            }
        }

    }
}
