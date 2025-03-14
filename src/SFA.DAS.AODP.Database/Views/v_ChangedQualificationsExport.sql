CREATE VIEW [regulated].[v_ChangedQualificationsExport] AS

/*##################################################################################################
	-Name:				Changed Qualifications Export
	-Description:		Gets qualification data to populate exported CSV file for changed qualifications, these are flagged in the Lifecycle Stage as 'Changed'
	-Date of Creation:	12/03/2025
	-Created By:		Robert Rybnikar
####################################################################################################

	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Robert Rybnikar		12/03/2025			Original
##################################################################################################*/

SELECT 
    -- Identifiers and Meta Information
    q.Qan AS QualificationNumber,   -- QAN as Qualification Number
    (SUBSTRING ( q.Qan, 1, 3 ) + '/' + SUBSTRING ( q.Qan, 4, 4 ) + '/' + SUBSTRING ( q.Qan, 8, 1 )) AS [QAN Text],
    GETDATE() AS [Date of download], -- Download timestamp
    -- Organisation Details
    ao.RecognitionNumber,
    ao.NameOfqual AS OrganisationName,
    ao.Acronym AS OrganisationAcronym,
    ao.Ukprn AS OrganisationReferenceNumber,
    -- Qualification Details
    q.QualificationName AS Title,
    qv.Type as QualificationType,
    qv.Level AS QualificationLevelCode,
    qv.SSA AS QualSSADescription,
    qv.SubLevel AS QualSSACode,
    -- Qualification Versions Details
    qv.LinkToSpecification,
    qv.TotalCredits AS QualCredit,
    qv.GradingType AS OverallGradingType,
    qv.GradingScale,
    qv.EntitlementFrameworkDesign AS EntitlementFrameworkDesignation,
    qv.Specialism,
    qv.Pathways,
    -- Region Availability
    qv.OfferedInEngland,
    NULL AS OfferedInWales,  -- Placeholder
    qv.OfferedInNi AS OfferedInNorthernIreland,
    -- Age Group Availability
    qv.PreSixteen,
    qv.SixteenToEighteen,
    qv.EighteenPlus,
    qv.NineteenPlus,
    -- Funding Details
    NULL AS FundingInEngland,          -- Placeholder
    NULL AS FundingInWales,            -- Placeholder
    NULL AS FundingInNorthernIreland,  -- Placeholder
    qv.GcseSizeEquivelence AS GCSESizeEquivalence,
    qv.GceSizeEquivelence AS GCESizeEquivalence,
    qv.Glh AS QualGLH,
    qv.MinimumGlh AS QualMinimumGLH,
    qv.MaximumGlh AS QualMaximumGLH,
    qv.Tqt AS TQT,
    qv.ApprovedForDelFundedProgramme AS ApprovedForDELFundedProgramme,
    qv.NiDiscountCode AS NIDiscountCode,
    -- Qualification Dates
    qv.RegulationStartDate,
    qv.OperationalStartDate,
    qv.ReviewDate,
    qv.OperationalEndDate,
    NULL AS EmbargoDate,  -- Placeholder
    qv.CertificationEndDate,
    -- Metadata & Versioning
    qv.UiLastUpdatedDate AS UILastUpdatedDate,
    qv.InsertedDate,
    qv.LastUpdatedDate,
    qv.Version,
    qv.OfferedInternationally
 
FROM dbo.Qualification q
Left Outer Join regulated.QualificationVersions qv ON q.Id = qv.QualificationId
Left Outer Join dbo.AwardingOrganisation ao ON qv.AwardingOrganisationId = ao.Id
Left Outer Join regulated.LifecycleStage ls ON qv.LifecycleStageId = ls.Id
 
WHERE ls.Name = 'Changed';
GO
