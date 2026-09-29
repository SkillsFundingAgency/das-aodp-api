CREATE VIEW [funded].[AllQualificationFundings] AS
SELECT
    qf.Id AS FundingId,
    CAST('Ofqual' AS NVARCHAR(20)) AS SourceType,
    qf.QualificationVersionId AS SourceQualificationId,
    q.Qan AS QualificationReference,
    qf.FundingOfferId,
    fo.Name AS FundingStreamName,
    qf.StartDate AS FundingApprovalStartDate,
    qf.EndDate AS FundingApprovalEndDate,
    CAST(NULL AS NVARCHAR(255)) AS FundingStatus,
    COALESCE(qv.Name, q.QualificationName) AS QualificationName,
    qv.Level,
    CAST(COALESCE(ao.NameOfqual, ao.NameLegal, ao.NameGovUk, ao.Name_Dsi, ao.Acronym) AS NVARCHAR(255)) AS AwardingOrganisationName
FROM [funded].[QualificationFundings] qf
INNER JOIN [regulated].[QualificationVersions] qv
    ON qv.Id = qf.QualificationVersionId
INNER JOIN [dbo].[FundingOffers] fo
    ON fo.Id = qf.FundingOfferId
LEFT JOIN [dbo].[Qualification] q
    ON q.Id = qv.QualificationId
LEFT JOIN [dbo].[AwardingOrganisation] ao
    ON ao.Id = qv.AwardingOrganisationId

UNION ALL

SELECT
    qaf.Id AS FundingId,
    CAST('QAA' AS NVARCHAR(20)) AS SourceType,
    qaf.QaaQualificationId AS SourceQualificationId,
    qaa.AimCode AS QualificationReference,
    qaf.FundingOfferId,
    fo.Name AS FundingStreamName,
    qaf.StartDate AS FundingApprovalStartDate,
    qaf.EndDate AS FundingApprovalEndDate,
    qaf.FundingStatus,
    qaa.QualificationTitle AS QualificationName,
    qaa.Level,
    qaa.AwardingBody AS AwardingOrganisationName
FROM [funded].[QaaQualificationFundings] qaf
INNER JOIN [regulated].[QaaQualification] qaa
    ON qaa.Id = qaf.QaaQualificationId
INNER JOIN [dbo].[FundingOffers] fo
    ON fo.Id = qaf.FundingOfferId;
GO
