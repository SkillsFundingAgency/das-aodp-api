using SFA.DAS.AODP.Data.Entities.Rollover;
using System.Text;

namespace SFA.DAS.AODP.Application.Services
{
    public interface IRolloverWorkflowCandidatesCsvBuilder
    {
        byte[] Build(IEnumerable<RolloverWorkflowCandidatesExportRow> rows);
    }

    public class RolloverWorkflowCandidatesCsvBuilder : IRolloverWorkflowCandidatesCsvBuilder
    {
        public byte[] Build(IEnumerable<RolloverWorkflowCandidatesExportRow> rows)
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Join(",", new[]
            {
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
        }));

            foreach (var r in rows)
            {
                sb.AppendLine(string.Join(",", new[]
                {
                Csv(r.QAN),
                Csv(r.QualificationTitle),
                Csv(r.AwardingOrganisation),
                Csv(r.QualificationLevel),
                Csv(r.QualificationType),
                Csv(r.SSA),
                Csv(r.OperationalEndDate),
                Csv(r.OfferedInEngland),
                Csv(r.FundedInEngland),
                Csv(r.GLH),
                Csv(r.TQT),
                Csv(r.Pre16),
                Csv(r.Age16To18),
                Csv(r.Age18Plus),
                Csv(r.Age19Plus),
                Csv(r.FundingStreamName),
                Csv(r.FundingApprovalStartDate),
                Csv(r.ProposedOutcome),
                Csv(r.RolloverStatus),
                Csv(r.ExclusionReason),
                Csv(r.CurrentFundingApprovalEndDate),
                Csv(r.ProposedFundingApprovalEndDate),
                Csv(r.Comments)
            }));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static string Csv(object? value)
        {
            if (value == null) return "";
            var s = value switch
            {
                DateTime dt => dt.ToString("yyyy-MM-dd"),
                bool b => b ? "True" : "False",
                _ => value.ToString() ?? ""
            };

            var needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            return needsQuotes ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
        }
    }
}
