using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services.Export;

public static class RolloverCandidateExportMapper
{

    public static void ApplyUserUpdates(
        RolloverCandidateForExport exportRow,
        RolloverCandidateForValidation uploaded)
    {
            exportRow.RowNumber = uploaded.RowNumber;
            exportRow.ProposedFundingApprovalEndDate = uploaded.ProposedFundingApprovalEndDate;
            exportRow.Comments = uploaded.Comments;
            exportRow.RolloverStatus = RolloverStatusInfo.FromCsv (uploaded.RollOverStatus);
            exportRow.ExclusionReason = uploaded.ExclusionReason;
    }
}
