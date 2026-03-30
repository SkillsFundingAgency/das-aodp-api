CREATE VIEW dbo.view_RolloverWorkflowCandidatesP1Checks
 AS
WITH Fundings AS (
    SELECT
        qf.QualificationVersionId,
        qf.FundingOfferId,
        MAX(qf.StartDate) AS LatestFundingApprovalStartDate,
        MAX(qf.EndDate)   AS LatestFundingApprovalEndDate
    FROM funded.QualificationFundings qf
    GROUP BY
        qf.QualificationVersionId,
        qf.FundingOfferId
),
DefLists AS (
    SELECT
        q.Id
    FROM dbo.DefundingLists dl
    INNER JOIN dbo.Qualification q
        ON q.Qan = dl.Qan
),
Pldns AS (
    SELECT
        q.Id,
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
    INNER JOIN dbo.Qualification q
        ON q.Qan = pldns.Qan
)
SELECT
    rwc.Id                     AS WorkflowCandidateId,
    rwc.RolloverWorkflowRunId,
    rwc.RolloverCandidatesId,
    rwc.QualificationVersionId,
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
    rwr.FundingEndDateEligibilityThreshold AS ThresholdDate,
    ff.LatestFundingApprovalEndDate,
    qv.OperationalStartDate,
    qv.OperationalEndDate,
    qv.OfferedInEngland,
    qv.Glh,
    qv.Tqt,
    CASE WHEN dl.Id IS NOT NULL THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsOnDefundingList,
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
INNER JOIN regulated.QualificationVersions qv
    ON qv.Id = rwc.QualificationVersionId
LEFT JOIN Fundings ff
    ON ff.QualificationVersionId = rwc.QualificationVersionId AND 
    ff.FundingOfferId = rwc.FundingOfferId

LEFT JOIN dbo.FundingOffers fo
        ON fo.Id = ff.FundingOfferId

   AND ff.FundingOfferId         = rwc.FundingOfferId
LEFT JOIN dbo.RolloverWorkflowRunFundingOffer rwfo
    ON rwfo.RolloverWorkflowRunId = rwc.RolloverWorkflowRunId
   AND rwfo.FundingOfferId        = rwc.FundingOfferId

LEFT JOIN DefLists dl
    ON dl.Id = qv.QualificationId

LEFT JOIN Pldns pldns
    ON pldns.Id = qv.QualificationId
GO