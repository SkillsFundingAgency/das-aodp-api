CREATE VIEW [regulated].[v_NewQualificationsExport] AS

/*##################################################################################################
	-Name:				New Qualifications Export
	-Description:		Gets qualification data to populate exported CSV file for new qualifications, these are flagged in the Lifecycle Stage as 'New'
	-Date of Creation:	20/02/2025
	-Created By:		Peter Roddy (Fujitsu)
####################################################################################################

	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Peter Roddy		20/02/2025			Original
##################################################################################################*/

SELECT 
    -- Identifiers and Meta Information
    qf.Qan AS QualificationNumber,   -- QAN as Qualification Number
    qf.Qan AS [QAN Text],            -- QAN reused as Text
    GETDATE() AS [Date of download], -- Download timestamp
    
    -- Organisation Details
    ao.RecognitionNumber,
    ao.NameLegal AS OrganisationName,
    ao.Acronym AS OrganisationAcronym,
    ao.Ukprn AS OrganisationReferenceNumber,
    
    -- Qualification Details
    qf.QualificationName AS Title,
    q.QualificationType,
    q.Level AS QualificationLevelCode,
    q.SectorSubjectArea AS QualSSADescription,
    q.SubCategory AS QualSSACode,
    
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

FROM funded.Qualifications q
JOIN dbo.AwardingOrganisation ao ON q.AwardingOrganisationId = ao.Id
JOIN regulated.QualificationVersions qv ON q.QualificationId = qv.QualificationId
JOIN dbo.Qualification qf ON q.QualificationId = qf.Id
JOIN regulated.LifecycleStage ls ON qv.LifecycleStageId = ls.Id
WHERE ls.Name = 'New';
