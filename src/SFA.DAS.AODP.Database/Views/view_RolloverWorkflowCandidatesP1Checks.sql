CREATE VIEW dbo.view_RolloverWorkflowCandidatesP1Checks
 AS
WITH Fundings AS (
    SELECT
        CAST('Ofqual' AS NVARCHAR(50)) AS SourceType,
        qf.QualificationVersionId AS SourceQualificationId,
        qf.FundingOfferId,
        MAX(qf.StartDate) AS LatestFundingApprovalStartDate,
        MAX(qf.EndDate) AS LatestFundingApprovalEndDate
    FROM funded.QualificationFundings qf
    GROUP BY
        qf.QualificationVersionId,
        qf.FundingOfferId

    UNION ALL

    SELECT
        CAST('QAA' AS NVARCHAR(50)) AS SourceType,
        qaf.QaaQualificationId AS SourceQualificationId,
        qaf.FundingOfferId,
        MAX(CAST(qaf.StartDate AS DATETIME2)) AS LatestFundingApprovalStartDate,
        MAX(CAST(qaf.EndDate AS DATETIME2)) AS LatestFundingApprovalEndDate
    FROM funded.QaaQualificationFundings qaf
    GROUP BY
        qaf.QaaQualificationId,
        qaf.FundingOfferId
),
DefLists AS (
    SELECT
        q.Qan
    FROM dbo.DefundingLists dl
    INNER JOIN dbo.Qualification q
        ON q.Qan = dl.Qan
),
Pldns AS (
    SELECT
        pldns.Qan,
        pldns.[PLDNS14-16],
        pldns.[PLDNS16-19],
        pldns.LocalFlex,
        pldns.[LegalEntitlementL2-L3],
        pldns.LegalEntitlementEngMaths,
        pldns.DigitalEntitlement,
        pldns.[ESF-L3-L4],
        pldns.Loans,
        pldns.LifelongLearningEntitlement,
        pldns.Level3FreeCoursesForJobs,
        pldns.CoF
    FROM dbo.Pldns pldns
),
SourceQualifications AS (
    SELECT
        CAST('Ofqual' AS NVARCHAR(50)) AS SourceType,
        qv.Id AS SourceQualificationId,
        q.Qan,
        qv.OperationalStartDate,
        qv.OperationalEndDate,
        qv.OfferedInEngland,
        qv.IntentionToSeekFundingInEngland
    FROM regulated.QualificationVersions qv
    INNER JOIN dbo.Qualification q
        ON q.Id = qv.QualificationId

    UNION ALL

    SELECT
        CAST('QAA' AS NVARCHAR(50)) AS SourceType,
        qaa.Id AS SourceQualificationId,
        qaa.AimCode AS Qan,
        CAST(qaa.StartDate AS DATETIME2) AS OperationalStartDate,
        CASE
            WHEN qaa.IsDiscontinued = 1 THEN CAST(qaa.DiscontinuedDate AS DATETIME2)
            ELSE NULL
        END AS OperationalEndDate,
        CAST(1 AS bit) AS OfferedInEngland,
        CAST(1 AS bit) AS IntentionToSeekFundingInEngland
    FROM regulated.QaaQualification qaa
)
SELECT
    rwc.Id                     AS WorkflowCandidateId,
    rwc.RolloverWorkflowRunId,
    rwc.RolloverCandidatesId,
    rwc.SourceType,
    rwc.SourceQualificationId,
    rwc.FundingOfferId,
    fo.Name AS FundingStream,
    rwc.AcademicYear,
    rwc.RolloverRound,
    rwc.CurrentFundingEndDate,
    rwc.ProposedFundingEndDate,
    rwc.IncludedInP1Export,
    rwc.IncludedInFinalUpload,
    rwc.CreatedAt,
    rwc.UpdatedAt,
    rwr.FundingEndDateEligibilityThreshold AS FundingEndDateThreshold,
    rwr.OperationalEndDateEligibilityThreshold AS OperationalEndDateThreshold,
    rwr.MaximumApprovalFundingEndDate AS MaximumApprovalEndDate,
    ff.LatestFundingApprovalEndDate,
    sq.OperationalStartDate,
    sq.OperationalEndDate,
    sq.OfferedInEngland,
    sq.IntentionToSeekFundingInEngland,
    CASE WHEN dl.Qan IS NOT NULL THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsOnDefundingList,
    pldns.[PLDNS14-16] AS Age1416,
    pldns.[PLDNS16-19] AS Age1619,
    pldns.LocalFlex AS LocalFlexibilities,
    pldns.[LegalEntitlementL2-L3] AS LegalEntitlementL2L3,
    pldns.LegalEntitlementEngMaths AS LegalEntitlementEnglishandMaths,
    pldns.DigitalEntitlement AS DigitalEntitlement,
    pldns.[ESF-L3-L4] AS ESFL3L4,
    pldns.Loans AS AdvancedLearnerLoans,
    pldns.LifelongLearningEntitlement AS LifelongLearningEntitlement,
    pldns.Level3FreeCoursesForJobs AS L3FreeCoursesForJobs,
    pldns.CoF

FROM dbo.RolloverWorkflowCandidate rwc
INNER JOIN dbo.RolloverWorkflowRun rwr
    ON rwr.Id = rwc.RolloverWorkflowRunId
INNER JOIN SourceQualifications sq
    ON sq.SourceType = rwc.SourceType
   AND sq.SourceQualificationId = rwc.SourceQualificationId
LEFT JOIN Fundings ff
   ON ff.SourceType = rwc.SourceType
   AND ff.SourceQualificationId = rwc.SourceQualificationId
   AND ff.FundingOfferId = rwc.FundingOfferId
LEFT JOIN dbo.FundingOffers fo
    ON fo.Id = rwc.FundingOfferId
LEFT JOIN dbo.RolloverWorkflowRunFundingOffer rwfo
    ON rwfo.RolloverWorkflowRunId = rwc.RolloverWorkflowRunId
   AND rwfo.FundingOfferId = rwc.FundingOfferId
LEFT JOIN DefLists dl
    ON dl.Qan = sq.Qan
LEFT JOIN Pldns pldns
    ON pldns.Qan = sq.Qan
WHERE rwc.InvalidatedAt IS NULL
GO
