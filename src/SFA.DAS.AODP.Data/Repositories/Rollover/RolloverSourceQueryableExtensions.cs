using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

internal static class RolloverSourceQueryableExtensions
{
    public static IQueryable<RolloverCandidateSourceProjection> WithSourceQualification(
        this IQueryable<RolloverCandidates> candidates,
        IApplicationDbContext context,
        string sourceType)
    {
        return sourceType switch
        {
            RolloverSourceTypes.Ofqual => candidates
                .Where(rc => rc.SourceType == RolloverSourceTypes.Ofqual)
                .Join(
                    context.QualificationVersions,
                    rc => rc.SourceQualificationId,
                    qv => qv.Id,
                    (rc, qv) => new RolloverCandidateSourceProjection
                    {
                        CandidateId = rc.Id,
                        SourceType = rc.SourceType,
                        SourceQualificationId = rc.SourceQualificationId,
                        FundingOfferId = rc.FundingOfferId,
                        FundingStreamName = rc.FundingOffer.Name,
                        QualificationReference = qv.Qualification.Qan,
                        QualificationTitle = qv.Qualification.QualificationName,
                        AwardingOrganisation = qv.Organisation.NameOfqual,
                        QualificationLevel = qv.Level,
                        QualificationType = qv.Type,
                        SectorSubjectArea = qv.Ssa,
                        AwardingOrganisationId = qv.AwardingOrganisationId,
                        AwardingOrganisationFilterId = qv.Organisation.RecognitionNumber,
                        AwardingOrganisationUkprn = qv.Organisation.Ukprn,
                        AwardingOrganisationRecognitionNumber = qv.Organisation.RecognitionNumber,
                        AwardingOrganisationNameLegal = qv.Organisation.NameLegal,
                        AwardingOrganisationNameGovUk = qv.Organisation.NameGovUk,
                        AwardingOrganisationNameDsi = qv.Organisation.Name_Dsi,
                        AwardingOrganisationAcronym = qv.Organisation.Acronym,
                        AcademicYear = rc.AcademicYear,
                        RolloverRound = rc.RolloverRound,
                        PreviousFundingEndDate = rc.PreviousFundingEndDate,
                        NewFundingEndDate = rc.NewFundingEndDate,
                        RolloverStatus = rc.RolloverStatus,
                        DiscussionQualificationId = qv.QualificationId,
                        OperationalStartDate = qv.OperationalStartDate,
                        OperationalEndDate = qv.OperationalEndDate,
                        OfferedInEngland = qv.OfferedInEngland,
                        IntentionToSeekFundingInEngland = qv.IntentionToSeekFundingInEngland ?? false,

                    }),

            RolloverSourceTypes.Qaa => candidates
                .Where(rc => rc.SourceType == RolloverSourceTypes.Qaa)
                .Join(
                    context.RegulatedQaaQualifications,
                    rc => rc.SourceQualificationId,
                    qaa => qaa.Id,
                    (rc, qaa) => new RolloverCandidateSourceProjection
                    {
                        CandidateId = rc.Id,
                        SourceType = rc.SourceType,
                        SourceQualificationId = rc.SourceQualificationId,
                        FundingOfferId = rc.FundingOfferId,
                        FundingStreamName = rc.FundingOffer.Name,
                        QualificationReference = qaa.AimCode,
                        QualificationTitle = qaa.QualificationTitle,
                        AwardingOrganisation = qaa.AwardingBody,
                        QualificationLevel = qaa.Level,
                        QualificationType = qaa.Type,
                        SectorSubjectArea = qaa.SectorSubjectAreaName,
                        AwardingOrganisationId = Guid.Empty,
                        AwardingOrganisationFilterId = qaa.AwardingBody,
                        AwardingOrganisationUkprn = null,
                        AwardingOrganisationRecognitionNumber = null,
                        AwardingOrganisationNameLegal = qaa.AwardingBody,
                        AwardingOrganisationNameGovUk = null,
                        AwardingOrganisationNameDsi = null,
                        AwardingOrganisationAcronym = null,
                        AcademicYear = rc.AcademicYear,
                        RolloverRound = rc.RolloverRound,
                        PreviousFundingEndDate = rc.PreviousFundingEndDate,
                        NewFundingEndDate = rc.NewFundingEndDate,
                        RolloverStatus = rc.RolloverStatus,
                        DiscussionQualificationId = (Guid?)null,
                        OperationalStartDate = null,
                        OperationalEndDate = null,
                        OfferedInEngland = true,
                        IntentionToSeekFundingInEngland = true,

                    }),

            _ => throw new NotSupportedException($"Unsupported rollover source type: {sourceType}")
        };
    }

    public static IQueryable<RolloverCandidateSourceProjection> WithAllSourceQualifications(
        this IQueryable<RolloverCandidates> candidates,
        IApplicationDbContext context)
    {
        return candidates
            .WithSourceQualification(context, RolloverSourceTypes.Ofqual)
            .Concat(candidates.WithSourceQualification(context, RolloverSourceTypes.Qaa));
    }

    public static IQueryable<RolloverWorkflowCandidateSourceProjection> WithSourceQualification(
        this IQueryable<RolloverWorkflowCandidate> workflowCandidates,
        IApplicationDbContext context,
        string sourceType)
    {
        return sourceType switch
        {
            RolloverSourceTypes.Ofqual => workflowCandidates
                .Where(rwc => rwc.SourceType == RolloverSourceTypes.Ofqual)
                .Join(
                    context.QualificationVersions,
                    rwc => rwc.SourceQualificationId,
                    qv => qv.Id,
                    (rwc, qv) => new RolloverWorkflowCandidateSourceProjection
                    {
                        WorkflowCandidateId = rwc.Id,
                        SourceType = rwc.SourceType,
                        SourceQualificationId = rwc.SourceQualificationId,
                        FundingOfferId = rwc.FundingOfferId,
                        FundingStreamName = rwc.RolloverCandidates.FundingOffer.Name,
                        QualificationReference = qv.Qualification.Qan,
                        QualificationTitle = qv.Qualification.QualificationName ?? string.Empty,
                        AwardingOrganisation = qv.Organisation.NameOfqual ?? string.Empty,
                        QualificationLevel = qv.Level,
                        QualificationType = qv.Type,
                        SSA = qv.Ssa,
                        OperationalEndDate = qv.OperationalEndDate,
                        OfferedInEngland = qv.OfferedInEngland,
                        FundedInEngland = qv.IntentionToSeekFundingInEngland ?? false,
                        GLH = qv.Glh,
                        TQT = qv.Tqt,
                        Pre16 = qv.PreSixteen ?? false,
                        Age16To18 = qv.SixteenToEighteen ?? false,
                        Age18Plus = qv.EighteenPlus ?? false,
                        Age19Plus = qv.NineteenPlus ?? false,
                        FundingApprovalStartDate = context.QualificationFundings
                            .Where(qf =>
                                qf.QualificationVersionId == rwc.SourceQualificationId &&
                                qf.FundingOfferId == rwc.FundingOfferId)
                            .Select(qf => qf.StartDate)
                            .FirstOrDefault(),
                        PassP1 = rwc.PassP1,
                        ExclusionReason = rwc.RolloverCandidates.ExclusionReason,
                        P1FailureReason = rwc.P1FailureReason,
                        CurrentFundingEndDate = rwc.CurrentFundingEndDate,
                        ProposedFundingEndDate = rwc.ProposedFundingEndDate
                    }),

            RolloverSourceTypes.Qaa => workflowCandidates
                .Where(rwc => rwc.SourceType == RolloverSourceTypes.Qaa)
                .Join(
                    context.RegulatedQaaQualifications,
                    rwc => rwc.SourceQualificationId,
                    qaa => qaa.Id,
                    (rwc, qaa) => new RolloverWorkflowCandidateSourceProjection
                    {
                        WorkflowCandidateId = rwc.Id,
                        SourceType = rwc.SourceType,
                        SourceQualificationId = rwc.SourceQualificationId,
                        FundingOfferId = rwc.FundingOfferId,
                        FundingStreamName = rwc.RolloverCandidates.FundingOffer.Name,
                        QualificationReference = qaa.AimCode,
                        QualificationTitle = qaa.QualificationTitle,
                        AwardingOrganisation = qaa.AwardingBody,
                        QualificationLevel = qaa.Level,
                        QualificationType = qaa.Type,
                        SSA = qaa.SectorSubjectAreaName,
                        OperationalEndDate = null,
                        OfferedInEngland = true,
                        FundedInEngland = true,
                        GLH = null,
                        TQT = null,
                        Pre16 = false,
                        Age16To18 = true,
                        Age18Plus = true,
                        Age19Plus = true,
                        FundingApprovalStartDate = context.QaaQualificationFundings
                            .Where(qf =>
                                qf.QaaQualificationId == rwc.SourceQualificationId &&
                                qf.FundingOfferId == rwc.FundingOfferId)
                            .Select(qf => qf.StartDate)
                            .FirstOrDefault(),
                        PassP1 = rwc.PassP1,
                        ExclusionReason = rwc.RolloverCandidates.ExclusionReason,
                        P1FailureReason = rwc.P1FailureReason,
                        CurrentFundingEndDate = rwc.CurrentFundingEndDate,
                        ProposedFundingEndDate = rwc.ProposedFundingEndDate
                    }),

            _ => throw new NotSupportedException($"Unsupported rollover source type: {sourceType}")
        };
    }

    public static IQueryable<RolloverWorkflowCandidateSourceProjection> WithAllSourceQualifications(
        this IQueryable<RolloverWorkflowCandidate> workflowCandidates,
        IApplicationDbContext context)
    {
        return workflowCandidates
            .WithSourceQualification(context, RolloverSourceTypes.Ofqual)
            .Concat(workflowCandidates.WithSourceQualification(context, RolloverSourceTypes.Qaa));
    }
}
