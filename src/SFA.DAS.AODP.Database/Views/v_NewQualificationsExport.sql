CREATE VIEW [funded].[v_NewQualificationsExport] AS

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
    
    -- Join with LifecycleStage to filter on 'New'
    qv.LinkToSpecification,
    qv.TotalCredits AS QualCredit,
    qv.GradingType AS OverallGradingType,
    qv.GradingScale,
    qv.EntitlementFrameworkDesign AS EntitlementFrameworkDesignation,
    qv.Specialism,
    qv.Pathways,
    
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

GO
