CREATE VIEW [dbo].[view_OutputUnchangedQualifications] AS

/*##################################################################################################
	-Name:				Output Unchanged Qualifications
	-Description:		All unchanged qualifications
	                    The must have a lifecycle stage of 'Completed'
						The latest qualification version must be used
	-Date of Creation:	10/04/2025
	-Created By:		Robert Rybnikar
####################################################################################################*/

WITH LatestQualificationGroup AS (
    SELECT
        ver.Id,
        ver.QualificationId,
        ver.AwardingOrganisationId,
        ver.Level AS Level,
        ver.Type AS QualificationType,
        ver.SubLevel AS Subcategory,
        ver.Ssa AS SectorSubjectArea,
        ver.InsertedTimestamp,
        ROW_NUMBER() OVER (PARTITION BY ver.QualificationId ORDER BY ver.Version DESC) AS rn
    FROM regulated.QualificationVersions ver
    WHERE ver.LifecycleStageId = '00000000-0000-0000-0000-000000000003' --completed
),
LatestQualifications AS (
    SELECT *
    FROM LatestQualificationGroup
    WHERE rn = 1
),
PivotFundingAvailable AS (
    SELECT
        qf.QualificationVersionId,
        MAX(CASE WHEN offertype.Name = 'LegalEntitlementEnglishandMaths' THEN 1 END) AS LegalEntitlementEnglishandMaths_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'LifelongLearningEntitlement' THEN 1 END) AS LifelongLearningEntitlement_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'LocalFlexibilities' THEN 1 END) AS LocalFlexibilities_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'DigitalEntitlement' THEN 1 END) AS DigitalEntitlement_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'LegalEntitlementL2L3' THEN 1 END) AS LegalEntitlementL2L3_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'AdvancedLearnerLoans' THEN 1 END) AS AdvancedLearnerLoans_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'Age1619' THEN 1 END) AS Age1619_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'L3FreeCoursesForJobs' THEN 1 END) AS L3FreeCoursesForJobs_FundingAvailable,
        MAX(CASE WHEN offertype.Name = 'Age1416' THEN 1 END) AS Age1416_FundingAvailable
    FROM funded.QualificationFundings qf
    INNER JOIN dbo.FundingOffers offertype ON offertype.Id = qf.FundingOfferId
    INNER JOIN LatestQualifications LQ ON qf.QualificationVersionId = LQ.Id
    GROUP BY qf.QualificationVersionId
),
PivotStartDate AS (
    SELECT
        qf.QualificationVersionId,
        MAX(CASE WHEN offertype.Name = 'LegalEntitlementEnglishandMaths' THEN StartDate END) AS LegalEntitlementEnglishandMaths_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'LifelongLearningEntitlement' THEN StartDate END) AS LifelongLearningEntitlement_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'LocalFlexibilities' THEN StartDate END) AS LocalFlexibilities_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'DigitalEntitlement' THEN StartDate END) AS DigitalEntitlement_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'LegalEntitlementL2L3' THEN StartDate END) AS LegalEntitlementL2L3_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'AdvancedLearnerLoans' THEN StartDate END) AS AdvancedLearnerLoans_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'Age1619' THEN StartDate END) AS Age1619_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'L3FreeCoursesForJobs' THEN StartDate END) AS L3FreeCoursesForJobs_FundingApprovalStartDate,
        MAX(CASE WHEN offertype.Name = 'Age1416' THEN StartDate END) AS Age1416_FundingApprovalStartDate
    FROM funded.QualificationFundings qf
    INNER JOIN dbo.FundingOffers offertype ON offertype.Id = qf.FundingOfferId
    INNER JOIN LatestQualifications LQ ON qf.QualificationVersionId = LQ.Id
    GROUP BY qf.QualificationVersionId
),
PivotEndDate AS (
    SELECT
        qf.QualificationVersionId,
        MAX(CASE WHEN offertype.Name = 'LegalEntitlementEnglishandMaths' THEN EndDate END) AS LegalEntitlementEnglishandMaths_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'LifelongLearningEntitlement' THEN EndDate END) AS LifelongLearningEntitlement_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'LocalFlexibilities' THEN EndDate END) AS LocalFlexibilities_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'DigitalEntitlement' THEN EndDate END) AS DigitalEntitlement_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'LegalEntitlementL2L3' THEN EndDate END) AS LegalEntitlementL2L3_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'AdvancedLearnerLoans' THEN EndDate END) AS AdvancedLearnerLoans_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'Age1619' THEN EndDate END) AS Age1619_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'L3FreeCoursesForJobs' THEN EndDate END) AS L3FreeCoursesForJobs_FundingApprovalEndDate,
        MAX(CASE WHEN offertype.Name = 'Age1416' THEN EndDate END) AS Age1416_FundingApprovalEndDate
    FROM funded.QualificationFundings qf
    INNER JOIN dbo.FundingOffers offertype ON offertype.Id = qf.FundingOfferId
    INNER JOIN LatestQualifications LQ ON qf.QualificationVersionId = LQ.Id
    GROUP BY qf.QualificationVersionId
),
CombinedPivotData AS (
    SELECT	    
        fa.QualificationVersionId,
        fa.AdvancedLearnerLoans_FundingAvailable,
        fs.AdvancedLearnerLoans_FundingApprovalStartDate,
        fe.AdvancedLearnerLoans_FundingApprovalEndDate,
        fa.Age1416_FundingAvailable,
        fs.Age1416_FundingApprovalStartDate,
        fe.Age1416_FundingApprovalEndDate,
        fa.Age1619_FundingAvailable,
        fs.Age1619_FundingApprovalStartDate,
        fe.Age1619_FundingApprovalEndDate,
        fa.DigitalEntitlement_FundingAvailable,
        fs.DigitalEntitlement_FundingApprovalStartDate,
        fe.DigitalEntitlement_FundingApprovalEndDate,
        fa.L3FreeCoursesForJobs_FundingAvailable,
        fs.L3FreeCoursesForJobs_FundingApprovalStartDate,
        fe.L3FreeCoursesForJobs_FundingApprovalEndDate,
        fa.LegalEntitlementEnglishandMaths_FundingAvailable,
        fs.LegalEntitlementEnglishandMaths_FundingApprovalStartDate,
        fe.LegalEntitlementEnglishandMaths_FundingApprovalEndDate,
        fa.LegalEntitlementL2L3_FundingAvailable,
        fs.LegalEntitlementL2L3_FundingApprovalStartDate,
        fe.LegalEntitlementL2L3_FundingApprovalEndDate,
        fa.LifelongLearningEntitlement_FundingAvailable,
        fs.LifelongLearningEntitlement_FundingApprovalStartDate,
        fe.LifelongLearningEntitlement_FundingApprovalEndDate,
        fa.LocalFlexibilities_FundingAvailable,
        fs.LocalFlexibilities_FundingApprovalStartDate,
        fe.LocalFlexibilities_FundingApprovalEndDate
    FROM PivotFundingAvailable fa
    JOIN PivotStartDate fs ON fa.QualificationVersionId = fs.QualificationVersionId
    JOIN PivotEndDate fe ON fa.QualificationVersionId = fe.QualificationVersionId
)
SELECT
	'Unchanged' AS Status,
    latestversion.InsertedTimeStamp AS DateOfOfqualDataSnapshot,
    qual.QualificationName AS Title,
    ao.NameOfqual AS OrganisationName,
    qual.Qan AS QualificationNumber,
    latestversion.Level,
    latestversion.QualificationType,
    latestversion.Subcategory,
    latestversion.SectorSubjectArea,
    pivotdata.AdvancedLearnerLoans_FundingAvailable,
    pivotdata.AdvancedLearnerLoans_FundingApprovalStartDate,
    pivotdata.AdvancedLearnerLoans_FundingApprovalEndDate,
    pivotdata.Age1416_FundingAvailable,
    pivotdata.Age1416_FundingApprovalStartDate,
    pivotdata.Age1416_FundingApprovalEndDate,
    pivotdata.Age1619_FundingAvailable,
    pivotdata.Age1619_FundingApprovalStartDate,
    pivotdata.Age1619_FundingApprovalEndDate,
    pivotdata.DigitalEntitlement_FundingAvailable,
    pivotdata.DigitalEntitlement_FundingApprovalStartDate,
    pivotdata.DigitalEntitlement_FundingApprovalEndDate,
    pivotdata.L3FreeCoursesForJobs_FundingAvailable,
    pivotdata.L3FreeCoursesForJobs_FundingApprovalStartDate,
    pivotdata.L3FreeCoursesForJobs_FundingApprovalEndDate,
    pivotdata.LegalEntitlementEnglishandMaths_FundingAvailable,
    pivotdata.LegalEntitlementEnglishandMaths_FundingApprovalStartDate,
    pivotdata.LegalEntitlementEnglishandMaths_FundingApprovalEndDate,
    pivotdata.LegalEntitlementL2L3_FundingAvailable,
    pivotdata.LegalEntitlementL2L3_FundingApprovalStartDate,
    pivotdata.LegalEntitlementL2L3_FundingApprovalEndDate,
    pivotdata.LifelongLearningEntitlement_FundingAvailable,
    pivotdata.LifelongLearningEntitlement_FundingApprovalStartDate,
    pivotdata.LifelongLearningEntitlement_FundingApprovalEndDate,
    pivotdata.LocalFlexibilities_FundingAvailable,
    pivotdata.LocalFlexibilities_FundingApprovalStartDate,
    pivotdata.LocalFlexibilities_FundingApprovalEndDate
FROM LatestQualifications latestversion
INNER JOIN dbo.Qualification qual ON qual.id = latestversion.QualificationId
INNER JOIN dbo.AwardingOrganisation ao ON ao.Id = latestversion.AwardingOrganisationId
LEFT JOIN CombinedPivotData pivotdata ON pivotdata.QualificationVersionId = latestversion.Id;
GO
