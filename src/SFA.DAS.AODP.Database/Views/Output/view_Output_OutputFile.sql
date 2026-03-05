CREATE VIEW [dbo].[view_Output_OutputFile] AS

/*##################################################################################################
    -Name:              Output File
    -Description:       View for combining the following output views:
                        - view_OutputChangedQualifications
                        - view_OutputNewQualifications
                        - view_OutputUnchangedQualifications
                        
    -Date of Creation:  08/10/2025
    -Created By:        Karen Hanna
####################################################################################################*/

SELECT 
      Status
    , DateOfOfqualDataSnapshot
    , Title AS QualificationName
    , OrganisationName AS AwardingOrganisation
    , AwardingOrganisationUrl AS AwardingOrganisationURL                            
    , QualificationNumber
    , Level
    , QualificationType
    , Subcategory
    , SectorSubjectArea

    , CAST(AdvancedLearnerLoans_FundingAvailable AS bit)      AS AdvancedLearnerLoans_FundingAvailable
    , AdvancedLearnerLoans_FundingApprovalStartDate
    , AdvancedLearnerLoans_FundingApprovalEndDate
    , AdvancedLearnerLoans_Notes

    , CAST(Age1416_FundingAvailable AS bit)                   AS Age1416_FundingAvailable
    , Age1416_FundingApprovalStartDate
    , Age1416_FundingApprovalEndDate
    , Age1416_Notes

    , CAST(Age1619_FundingAvailable AS bit)                   AS Age1619_FundingAvailable
    , Age1619_FundingApprovalStartDate
    , Age1619_FundingApprovalEndDate
    , Age1619_Notes

    , CAST(DigitalEntitlement_FundingAvailable AS bit)         AS DigitalEntitlement_FundingAvailable
    , DigitalEntitlement_FundingApprovalStartDate
    , DigitalEntitlement_FundingApprovalEndDate
    , DigitalEntitlement_Notes

    , CAST(L3FreeCoursesForJobs_FundingAvailable AS bit)       AS FreeCoursesForJobs_FundingAvailable
    , L3FreeCoursesForJobs_FundingApprovalStartDate            AS FreeCoursesForJobs_FundingApprovalStartDate
    , L3FreeCoursesForJobs_FundingApprovalEndDate              AS FreeCoursesForJobs_FundingApprovalEndDate
    , L3FreeCoursesForJobs_Notes                               AS FreeCoursesForJobs_Notes

    , CAST(LegalEntitlementEnglishandMaths_FundingAvailable AS bit) AS LegalEntitlementEnglishandMaths_FundingAvailable
    , LegalEntitlementEnglishandMaths_FundingApprovalStartDate
    , LegalEntitlementEnglishandMaths_FundingApprovalEndDate
    , LegalEntitlementEnglishandMaths_Notes

    , CAST(LegalEntitlementL2L3_FundingAvailable AS bit)       AS LegalEntitlementL2L3_FundingAvailable
    , LegalEntitlementL2L3_FundingApprovalStartDate
    , LegalEntitlementL2L3_FundingApprovalEndDate
    , LegalEntitlementL2L3_Notes

    , CAST(LifelongLearningEntitlement_FundingAvailable AS bit) AS LifelongLearningEntitlement_FundingAvailable
    , LifelongLearningEntitlement_FundingApprovalStartDate
    , LifelongLearningEntitlement_FundingApprovalEndDate
    , LifelongLearningEntitlement_Notes

    , CAST(LocalFlexibilities_FundingAvailable AS bit)         AS LocalFlexibilities_FundingAvailable
    , LocalFlexibilities_FundingApprovalStartDate
    , LocalFlexibilities_FundingApprovalEndDate
    , LocalFlexibilities_Notes
FROM dbo.view_OutputChangedQualifications

UNION

SELECT 
      Status
    , DateOfOfqualDataSnapshot
    , Title AS QualificationName
    , OrganisationName AS AwardingOrganisation
    , AwardingOrganisationUrl                             
    , QualificationNumber
    , Level
    , QualificationType
    , Subcategory
    , SectorSubjectArea
    , CAST(AdvancedLearnerLoans_FundingAvailable AS bit)      AS AdvancedLearnerLoans_FundingAvailable
    , AdvancedLearnerLoans_FundingApprovalStartDate
    , AdvancedLearnerLoans_FundingApprovalEndDate
    , AdvancedLearnerLoans_Notes
    , CAST(Age1416_FundingAvailable AS bit)                   AS Age1416_FundingAvailable
    , Age1416_FundingApprovalStartDate
    , Age1416_FundingApprovalEndDate
    , Age1416_Notes
    , CAST(Age1619_FundingAvailable AS bit)                   AS Age1619_FundingAvailable
    , Age1619_FundingApprovalStartDate
    , Age1619_FundingApprovalEndDate
    , Age1619_Notes
    , CAST(DigitalEntitlement_FundingAvailable AS bit)         AS DigitalEntitlement_FundingAvailable
    , DigitalEntitlement_FundingApprovalStartDate
    , DigitalEntitlement_FundingApprovalEndDate
    , DigitalEntitlement_Notes
    , CAST(L3FreeCoursesForJobs_FundingAvailable AS bit)       AS FreeCoursesForJobs_FundingAvailable
    , L3FreeCoursesForJobs_FundingApprovalStartDate            AS FreeCoursesForJobs_FundingApprovalStartDate
    , L3FreeCoursesForJobs_FundingApprovalEndDate              AS FreeCoursesForJobs_FundingApprovalEndDate
    , L3FreeCoursesForJobs_Notes                               AS FreeCoursesForJobs_Notes
    , CAST(LegalEntitlementEnglishandMaths_FundingAvailable AS bit) AS LegalEntitlementEnglishandMaths_FundingAvailable
    , LegalEntitlementEnglishandMaths_FundingApprovalStartDate
    , LegalEntitlementEnglishandMaths_FundingApprovalEndDate
    , LegalEntitlementEnglishandMaths_Notes
    , CAST(LegalEntitlementL2L3_FundingAvailable AS bit)       AS LegalEntitlementL2L3_FundingAvailable
    , LegalEntitlementL2L3_FundingApprovalStartDate
    , LegalEntitlementL2L3_FundingApprovalEndDate
    , LegalEntitlementL2L3_Notes
    , CAST(LifelongLearningEntitlement_FundingAvailable AS bit) AS LifelongLearningEntitlement_FundingAvailable
    , LifelongLearningEntitlement_FundingApprovalStartDate
    , LifelongLearningEntitlement_FundingApprovalEndDate
    , LifelongLearningEntitlement_Notes
    , CAST(LocalFlexibilities_FundingAvailable AS bit)         AS LocalFlexibilities_FundingAvailable
    , LocalFlexibilities_FundingApprovalStartDate
    , LocalFlexibilities_FundingApprovalEndDate
    , LocalFlexibilities_Notes
FROM dbo.view_OutputNewQualifications

UNION

SELECT 
      Status
    , DateOfOfqualDataSnapshot
    , Title AS QualificationName
    , OrganisationName AS AwardingOrganisation
    , AwardingOrganisationUrl                            
    , QualificationNumber
    , Level
    , QualificationType
    , Subcategory
    , SectorSubjectArea
    , CAST(AdvancedLearnerLoans_FundingAvailable AS bit)      AS AdvancedLearnerLoans_FundingAvailable
    , AdvancedLearnerLoans_FundingApprovalStartDate
    , AdvancedLearnerLoans_FundingApprovalEndDate
    , AdvancedLearnerLoans_Notes
    , CAST(Age1416_FundingAvailable AS bit)                   AS Age1416_FundingAvailable
    , Age1416_FundingApprovalStartDate
    , Age1416_FundingApprovalEndDate
    , Age1416_Notes
    , CAST(Age1619_FundingAvailable AS bit)                   AS Age1619_FundingAvailable
    , Age1619_FundingApprovalStartDate
    , Age1619_FundingApprovalEndDate
    , Age1619_Notes
    , CAST(DigitalEntitlement_FundingAvailable AS bit)         AS DigitalEntitlement_FundingAvailable
    , DigitalEntitlement_FundingApprovalStartDate
    , DigitalEntitlement_FundingApprovalEndDate
    , DigitalEntitlement_Notes
    , CAST(L3FreeCoursesForJobs_FundingAvailable AS bit)       AS FreeCoursesForJobs_FundingAvailable
    , L3FreeCoursesForJobs_FundingApprovalStartDate            AS FreeCoursesForJobs_FundingApprovalStartDate
    , L3FreeCoursesForJobs_FundingApprovalEndDate              AS FreeCoursesForJobs_FundingApprovalEndDate
    , L3FreeCoursesForJobs_Notes                               AS FreeCoursesForJobs_Notes
    , CAST(LegalEntitlementEnglishandMaths_FundingAvailable AS bit) AS LegalEntitlementEnglishandMaths_FundingAvailable
    , LegalEntitlementEnglishandMaths_FundingApprovalStartDate
    , LegalEntitlementEnglishandMaths_FundingApprovalEndDate
    , LegalEntitlementEnglishandMaths_Notes
    , CAST(LegalEntitlementL2L3_FundingAvailable AS bit)       AS LegalEntitlementL2L3_FundingAvailable
    , LegalEntitlementL2L3_FundingApprovalStartDate
    , LegalEntitlementL2L3_FundingApprovalEndDate
    , LegalEntitlementL2L3_Notes
    , CAST(LifelongLearningEntitlement_FundingAvailable AS bit) AS LifelongLearningEntitlement_FundingAvailable
    , LifelongLearningEntitlement_FundingApprovalStartDate
    , LifelongLearningEntitlement_FundingApprovalEndDate
    , LifelongLearningEntitlement_Notes
    , CAST(LocalFlexibilities_FundingAvailable AS bit)         AS LocalFlexibilities_FundingAvailable
    , LocalFlexibilities_FundingApprovalStartDate
    , LocalFlexibilities_FundingApprovalEndDate
    , LocalFlexibilities_Notes
FROM dbo.view_OutputUnchangedQualifications

UNION

SELECT 
      Status
    , DateOfDataSnapshot
    , QualificationTitle                   AS QualificationName
    , AwardingBody                         AS AwardingOrganisation
    , NULL                                 AS AwardingOrganisationURL                            
    , AimCode                              AS QualificationNumber
    , Level
    , Type                                 AS QualificationType
    , NULL                                 AS Subcategory
    , SectorSubjectArea

    , CAST(1 AS bit)                       AS AdvancedLearnerLoans_FundingAvailable
    , StartDate                            AS AdvancedLearnerLoans_FundingApprovalStartDate
    , LastFundingApprovalEndDate           AS AdvancedLearnerLoans_FundingApprovalEndDate
    , NULL                                 AS AdvancedLearnerLoans_Notes

    , CAST(0 AS bit)                       AS Age1416_FundingAvailable
    , NULL                                 AS Age1416_FundingApprovalStartDate
    , NULL                                 AS Age1416_FundingApprovalEndDate
    , NULL                                 AS Age1416_Notes

    , CAST(1 AS bit)                       AS Age1619_FundingAvailable
    , StartDate                            AS Age1619_FundingApprovalStartDate
    , LastFundingApprovalEndDate           AS Age1619_FundingApprovalEndDate
    , NULL                                 AS Age1619_Notes

    , CAST(0 AS bit)                       AS DigitalEntitlement_FundingAvailable
    , NULL                                 AS DigitalEntitlement_FundingApprovalStartDate
    , NULL                                 AS DigitalEntitlement_FundingApprovalEndDate
    , NULL                                 AS DigitalEntitlement_Notes

    , CAST(0 AS bit)                       AS FreeCoursesForJobs_FundingAvailable
    , NULL                                 AS FreeCoursesForJobs_FundingApprovalStartDate
    , NULL                                 AS FreeCoursesForJobs_FundingApprovalEndDate
    , NULL                                 AS FreeCoursesForJobs_Notes

    , CAST(0 AS bit)                       AS LegalEntitlementEnglishandMaths_FundingAvailable
    , NULL                                 AS LegalEntitlementEnglishandMaths_FundingApprovalStartDate
    , NULL                                 AS LegalEntitlementEnglishandMaths_FundingApprovalEndDate
    , NULL                                 AS LegalEntitlementEnglishandMaths_Notes

    , CAST(1 AS bit)                       AS LegalEntitlementL2L3_FundingAvailable
    , StartDate                            AS LegalEntitlementL2L3_FundingApprovalStartDate
    , LastFundingApprovalEndDate           AS LegalEntitlementL2L3_FundingApprovalEndDate
    , NULL                                 AS LegalEntitlementL2L3_Notes

    , CAST(0 AS bit)                       AS LifelongLearningEntitlement_FundingAvailable
    , NULL                                 AS LifelongLearningEntitlement_FundingApprovalStartDate
    , NULL                                 AS LifelongLearningEntitlement_FundingApprovalEndDate
    , NULL                                 AS LifelongLearningEntitlement_Notes

    , CAST(0 AS bit)                       AS LocalFlexibilities_FundingAvailable
    , NULL                                 AS LocalFlexibilities_FundingApprovalStartDate
    , NULL                                 AS LocalFlexibilities_FundingApprovalEndDate
    , NULL                                 AS LocalFlexibilities_Notes
FROM dbo.view_OutputQaaQualifications;

GO