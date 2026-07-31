CREATE VIEW [dbo].[view_OutputFundedQualifications] AS

/*##################################################################################################
	-Name:			    Output Funded Qualifications
	-Description:		All qualifications that have been funded during the current review cycle
                        The latest qualification version must be used
	-Date of Creation:	19/04/2026
	-Created By:		Hamzah Shakeel

    Modification History
    ------------------------------------------------------------------------------------------------
    Date        Author          Description
    ----------  --------------  --------------------------------------------------------------
    31/07/2026  Karen Hanna     Optimised funding aggregation by replacing multiple pivot
                                CTEs with a single FundingData CTE. Moved funding notes
                                into a separate FundingNotes CTE and filtered NULL/empty
                                comments before aggregation to improve query performance.

####################################################################################################*/

WITH LatestQualificationGroup AS
(
    SELECT
        ver.Id,
        ver.QualificationId,
        ver.AwardingOrganisationId,
        ver.Level,
        ver.Type AS QualificationType,
        ver.SubLevel AS Subcategory,
        ver.Ssa AS SectorSubjectArea,
        ver.InsertedTimestamp,
        ROW_NUMBER() OVER
        (
            PARTITION BY ver.QualificationId
            ORDER BY ver.Version DESC
        ) AS rn
    FROM regulated.QualificationVersions ver
    WHERE EXISTS
    (
        SELECT 1
        FROM funded.QualificationFundings qf
        WHERE qf.QualificationVersionId = ver.Id
    )
),
LatestQualifications AS
(
    SELECT
        Id,
        QualificationId,
        AwardingOrganisationId,
        Level,
        QualificationType,
        Subcategory,
        SectorSubjectArea,
        InsertedTimestamp
    FROM LatestQualificationGroup
    WHERE rn = 1
),
FundingData AS
(
    SELECT
        qf.QualificationVersionId,

        MAX
        (
            CASE
                WHEN fo.Name = 'AdvancedLearnerLoans'
                THEN 1
                ELSE 0
            END
        ) AS AdvancedLearnerLoans_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'AdvancedLearnerLoans'
                THEN qf.StartDate
            END
        ) AS AdvancedLearnerLoans_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'AdvancedLearnerLoans'
                THEN qf.EndDate
            END
        ) AS AdvancedLearnerLoans_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'Age1416'
                THEN 1
                ELSE 0
            END
        ) AS Age1416_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'Age1416'
                THEN qf.StartDate
            END
        ) AS Age1416_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'Age1416'
                THEN qf.EndDate
            END
        ) AS Age1416_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'Age1619'
                THEN 1
                ELSE 0
            END
        ) AS Age1619_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'Age1619'
                THEN qf.StartDate
            END
        ) AS Age1619_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'Age1619'
                THEN qf.EndDate
            END
        ) AS Age1619_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'DigitalEntitlement'
                THEN 1
                ELSE 0
            END
        ) AS DigitalEntitlement_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'DigitalEntitlement'
                THEN qf.StartDate
            END
        ) AS DigitalEntitlement_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'DigitalEntitlement'
                THEN qf.EndDate
            END
        ) AS DigitalEntitlement_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'L3FreeCoursesForJobs'
                THEN 1
                ELSE 0
            END
        ) AS L3FreeCoursesForJobs_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'L3FreeCoursesForJobs'
                THEN qf.StartDate
            END
        ) AS L3FreeCoursesForJobs_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'L3FreeCoursesForJobs'
                THEN qf.EndDate
            END
        ) AS L3FreeCoursesForJobs_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementEnglishandMaths'
                THEN 1
                ELSE 0
            END
        ) AS LegalEntitlementEnglishandMaths_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementEnglishandMaths'
                THEN qf.StartDate
            END
        ) AS LegalEntitlementEnglishandMaths_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementEnglishandMaths'
                THEN qf.EndDate
            END
        ) AS LegalEntitlementEnglishandMaths_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementL2L3'
                THEN 1
                ELSE 0
            END
        ) AS LegalEntitlementL2L3_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementL2L3'
                THEN qf.StartDate
            END
        ) AS LegalEntitlementL2L3_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementL2L3'
                THEN qf.EndDate
            END
        ) AS LegalEntitlementL2L3_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'LifelongLearningEntitlement'
                THEN 1
                ELSE 0
            END
        ) AS LifelongLearningEntitlement_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'LifelongLearningEntitlement'
                THEN qf.StartDate
            END
        ) AS LifelongLearningEntitlement_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'LifelongLearningEntitlement'
                THEN qf.EndDate
            END
        ) AS LifelongLearningEntitlement_FundingApprovalEndDate,

        MAX
        (
            CASE
                WHEN fo.Name = 'LocalFlexibilities'
                THEN 1
                ELSE 0
            END
        ) AS LocalFlexibilities_FundingAvailable,
        MAX
        (
            CASE
                WHEN fo.Name = 'LocalFlexibilities'
                THEN qf.StartDate
            END
        ) AS LocalFlexibilities_FundingApprovalStartDate,
        MAX
        (
            CASE
                WHEN fo.Name = 'LocalFlexibilities'
                THEN qf.EndDate
            END
        ) AS LocalFlexibilities_FundingApprovalEndDate

    FROM funded.QualificationFundings qf
    INNER JOIN dbo.FundingOffers fo
        ON fo.Id = qf.FundingOfferId
    INNER JOIN LatestQualifications lq
        ON lq.Id = qf.QualificationVersionId
    GROUP BY
        qf.QualificationVersionId
),
FundingNotes AS
(
    SELECT
        qf.QualificationVersionId,

        MAX
        (
            CASE
                WHEN fo.Name = 'AdvancedLearnerLoans'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS AdvancedLearnerLoans_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'Age1416'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS Age1416_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'Age1619'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS Age1619_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'DigitalEntitlement'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS DigitalEntitlement_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'L3FreeCoursesForJobs'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS L3FreeCoursesForJobs_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementEnglishandMaths'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS LegalEntitlementEnglishandMaths_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'LegalEntitlementL2L3'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS LegalEntitlementL2L3_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'LifelongLearningEntitlement'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS LifelongLearningEntitlement_Notes,

        MAX
        (
            CASE
                WHEN fo.Name = 'LocalFlexibilities'
                THEN CONVERT(nvarchar(500), qf.Comments)
            END
        ) AS LocalFlexibilities_Notes

    FROM funded.QualificationFundings qf
    INNER JOIN dbo.FundingOffers fo
        ON fo.Id = qf.FundingOfferId
    INNER JOIN LatestQualifications lq
        ON lq.Id = qf.QualificationVersionId

    WHERE qf.Comments IS NOT NULL
      AND qf.Comments <> N''

    GROUP BY
        qf.QualificationVersionId
)
SELECT
    'ApprovedCompleted' AS Status,
    latestversion.InsertedTimestamp AS DateOfOfqualDataSnapshot,
    qual.QualificationName AS Title,
    ao.NameOfqual AS OrganisationName,
    qual.Qan AS QualificationNumber,
    latestversion.Level,
    latestversion.QualificationType,
    fq.Subcategory,
    latestversion.SectorSubjectArea,

    funding.AdvancedLearnerLoans_FundingAvailable,
    funding.AdvancedLearnerLoans_FundingApprovalStartDate,
    funding.AdvancedLearnerLoans_FundingApprovalEndDate,
    notes.AdvancedLearnerLoans_Notes,

    funding.Age1416_FundingAvailable,
    funding.Age1416_FundingApprovalStartDate,
    funding.Age1416_FundingApprovalEndDate,
    notes.Age1416_Notes,

    funding.Age1619_FundingAvailable,
    funding.Age1619_FundingApprovalStartDate,
    funding.Age1619_FundingApprovalEndDate,
    notes.Age1619_Notes,

    funding.DigitalEntitlement_FundingAvailable,
    funding.DigitalEntitlement_FundingApprovalStartDate,
    funding.DigitalEntitlement_FundingApprovalEndDate,
    notes.DigitalEntitlement_Notes,

    funding.L3FreeCoursesForJobs_FundingAvailable,
    funding.L3FreeCoursesForJobs_FundingApprovalStartDate,
    funding.L3FreeCoursesForJobs_FundingApprovalEndDate,
    notes.L3FreeCoursesForJobs_Notes,

    funding.LegalEntitlementEnglishandMaths_FundingAvailable,
    funding.LegalEntitlementEnglishandMaths_FundingApprovalStartDate,
    funding.LegalEntitlementEnglishandMaths_FundingApprovalEndDate,
    notes.LegalEntitlementEnglishandMaths_Notes,

    funding.LegalEntitlementL2L3_FundingAvailable,
    funding.LegalEntitlementL2L3_FundingApprovalStartDate,
    funding.LegalEntitlementL2L3_FundingApprovalEndDate,
    notes.LegalEntitlementL2L3_Notes,

    funding.LifelongLearningEntitlement_FundingAvailable,
    funding.LifelongLearningEntitlement_FundingApprovalStartDate,
    funding.LifelongLearningEntitlement_FundingApprovalEndDate,
    notes.LifelongLearningEntitlement_Notes,

    funding.LocalFlexibilities_FundingAvailable,
    funding.LocalFlexibilities_FundingApprovalStartDate,
    funding.LocalFlexibilities_FundingApprovalEndDate,
    notes.LocalFlexibilities_Notes,

    fq.AwardingOrganisationUrl
FROM LatestQualifications latestversion

INNER JOIN dbo.Qualification qual
    ON qual.Id = latestversion.QualificationId

INNER JOIN dbo.AwardingOrganisation ao
    ON ao.Id = latestversion.AwardingOrganisationId

LEFT JOIN FundingData funding
    ON funding.QualificationVersionId = latestversion.Id

LEFT JOIN FundingNotes notes
    ON notes.QualificationVersionId = latestversion.Id

LEFT JOIN funded.Qualifications fq
    ON fq.QualificationId = qual.Id;
GO