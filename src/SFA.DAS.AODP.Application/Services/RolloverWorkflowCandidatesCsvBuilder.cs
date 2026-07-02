using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Infrastructure.Extensions;
using System.Text;

namespace SFA.DAS.AODP.Application.Services
{
    public interface IRolloverWorkflowCandidatesCsvBuilder
    {
        byte[] Build(IEnumerable<RolloverWorkflowCandidatesExportRow> rows);
    }

    public class RolloverWorkflowCandidatesCsvBuilder : IRolloverWorkflowCandidatesCsvBuilder
    {
        private static readonly string[] Columns =
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

        public byte[] Build(IEnumerable<RolloverWorkflowCandidatesExportRow> rows)
        {
            using var ms = new MemoryStream(4096);

            using (var writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true))
            {
                writer.WriteLine(string.Join(",", Columns));

                foreach (var r in rows)
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

                    writer.WriteLine(); 
                }
            }

            return ms.ToArray();
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
