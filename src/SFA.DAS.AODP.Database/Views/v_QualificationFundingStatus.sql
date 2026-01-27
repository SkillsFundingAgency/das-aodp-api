CREATE VIEW [regulated].[v_QualificationFundingStatus] AS

/*##################################################################################################
	-Name:				Qualifications Funding Status
	-Description:		Returns the latest version of each regulated qualification with its derived funding status and related funded snapshot data.
	-Date of Creation:	21/01/2026
	-Created By:		Adeel Dar
####################################################################################################

	Version No.			Updated By		Updated Date		Description of Change
#####################################################################################################
	1					Adeel DAr		21/01/2026			Original
##################################################################################################*/

WITH LatestQualificationVersions AS (
    SELECT
        qv.Id,
        qv.QualificationId,
        qv.Name,
        qv.Level,
        qv.Type,
        qv.Ssa,
        qv.SixteenToEighteen,
        qv.NineteenPlus,
        qv.PreSixteen,
        qv.EighteenPlus,
        qv.OperationalStartDate,
        qv.OperationalEndDate,
        qv.ProcessStatusId AS FundedStatus,
        CAST(qv.EligibleForFunding AS BIT)              AS EligibleForFunding,
        CAST(COALESCE(qv.OfferedInEngland, 0) AS BIT)   AS OfferedInEnglandFlag,
        qv.AwardingOrganisationId,
        qv.Version,
        qv.LastUpdatedDate,
        ROW_NUMBER() OVER (
            PARTITION BY qv.QualificationId
            ORDER BY qv.Version DESC, qv.LastUpdatedDate DESC
        ) AS rn
    FROM [regulated].[QualificationVersions] AS qv
),
Classified AS (
    SELECT
        lv.Id                       AS QualificationVersionId,
        lv.QualificationId,
        q.Qan,
        lv.Name                     AS QualificationName,
        lv.Level,
        lv.Type,
        lv.Ssa,
        lv.SixteenToEighteen,
        lv.NineteenPlus,
        lv.PreSixteen,
        lv.EighteenPlus,
        lv.OperationalStartDate,
        lv.OperationalEndDate,
        lv.EligibleForFunding,
        lv.FundedStatus,
        CASE WHEN lv.OfferedInEnglandFlag = 1
             THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS FundedInEngland,
        fq.Id                      AS FundedQualificationSnapshotId,
        fq.Level                   AS FundedLevel,
        fq.QualificationType       AS FundedQualificationType,
        fq.SubCategory,
        fq.SectorSubjectArea,
        fq.DateOfOfqualDataSnapshot,
        ao.NameOfqual              AS AwardingOrganisationName,
        CASE
            WHEN lv.EligibleForFunding = 1
                 AND lv.OfferedInEnglandFlag = 1
                 AND lv.OperationalStartDate <= CAST(GETDATE() AS date)
                 AND (
                        lv.OperationalEndDate IS NULL
                        OR lv.OperationalEndDate >= CAST(GETDATE() AS date)
                 )
            THEN 'CurrentlyFunded'
            ELSE 'Unfunded'
        END AS FundingStatus
    FROM LatestQualificationVersions AS lv
    LEFT JOIN [funded].[Qualifications]    AS fq ON fq.QualificationId = lv.QualificationId
    LEFT JOIN [dbo].[Qualification]        AS q  ON q.Id               = lv.QualificationId
    LEFT JOIN [dbo].[AwardingOrganisation] AS ao ON ao.Id              = lv.AwardingOrganisationId
    WHERE lv.rn = 1
)
SELECT
    QualificationVersionId,
    QualificationId,
    Qan,
    QualificationName,
    Level,
    Type,
    Ssa,
    SixteenToEighteen,
    NineteenPlus,
    PreSixteen,
    EighteenPlus,
    OperationalStartDate,
    OperationalEndDate,
    EligibleForFunding,
    FundedInEngland,
    FundedQualificationSnapshotId,
    FundedLevel,
    FundedQualificationType,
    SubCategory,
    SectorSubjectArea,
    FundedStatus,
    DateOfOfqualDataSnapshot,
    AwardingOrganisationName,
    FundingStatus
FROM Classified;
GO