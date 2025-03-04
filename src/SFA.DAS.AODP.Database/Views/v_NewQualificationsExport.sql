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
    qf.Qan AS QualificationNumber,   
    qf.Qan AS [QAN Text],            
    GETDATE() AS [Date of download], 

    -- Organisation Details
    ao.RecognitionNumber,
    ao.NameOfqual AS OrganisationName,
    ao.Acronym AS OrganisationAcronym,
    ao.Ukprn AS OrganisationReferenceNumber,

    -- Qualification Details
    qf.QualificationName AS Title,
    qv.LinkToSpecification,
    q.[Level] AS QualificationLevelCode,
    q.QualificationType,
    qv.TotalCredits AS QualCredit,
    q.SubCategory AS QualSSACode,
    q.SectorSubjectArea AS QualSSADescription,
    qv.GradingType AS OverallGradingType,
    qv.GradingScale,
    qv.EntitlementFrameworkDesign AS EntitlementFrameworkDesignation,
    qv.Specialism,
    qv.Pathways,

    -- Region Availability
    qv.OfferedInEngland,
    NULL AS OfferedInWales,  
    qv.OfferedInNi AS OfferedInNorthernIreland,

    -- Age Group Availability
    qv.PreSixteen,
    qv.SixteenToEighteen,
    qv.EighteenPlus,
    qv.NineteenPlus,

    -- Funding Details
    NULL AS FundingInEngland,          
    NULL AS FundingInWales,            
    NULL AS FundingInNorthernIreland,  
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
    NULL AS EmbargoDate,  
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
