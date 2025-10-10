CREATE View [dbo].[view_Output_ExportFile] as

/*##################################################################################################
	-Name:				Output Export File
	-Description:		View for combining the following output views:
						- view_OutputChangedQualifications
						- view_OutputNewQualifications
						- view_OutputUnchangedQualifications
						
	-Date of Creation:	08/10/2025
	-Created By:		Karen Hanna
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Karen Hanna 	08/10/2025			Original
##################################################################################################*/

select Status
      ,DateOfOfqualDataSnapshot
      ,Title as QualificationName
      ,OrganisationName AS AwardingOrganisation
      ,QualificationNumber
      ,Level
      ,QualificationType
      ,Subcategory
      ,SectorSubjectArea
      ,AdvancedLearnerLoans_FundingAvailable
      ,AdvancedLearnerLoans_FundingApprovalStartDate
      ,AdvancedLearnerLoans_FundingApprovalEndDate
      ,Age1416_FundingAvailable
      ,Age1416_FundingApprovalStartDate
      ,Age1416_FundingApprovalEndDate
      ,Age1416_Notes
      ,Age1619_FundingAvailable
      ,Age1619_FundingApprovalStartDate
      ,Age1619_FundingApprovalEndDate
      ,Age1619_Notes
      ,DigitalEntitlement_FundingAvailable
      ,DigitalEntitlement_FundingApprovalStartDate
      ,DigitalEntitlement_FundingApprovalEndDate
      ,DigitalEntitlement_Notes
      ,L3FreeCoursesForJobs_FundingAvailable
      ,L3FreeCoursesForJobs_FundingApprovalStartDate
      ,L3FreeCoursesForJobs_FundingApprovalEndDate
      ,L3FreeCoursesForJobs_Notes
      ,LegalEntitlementEnglishandMaths_FundingAvailable
      ,LegalEntitlementEnglishandMaths_FundingApprovalStartDate
      ,LegalEntitlementEnglishandMaths_FundingApprovalEndDate
      ,LegalEntitlementEnglishandMaths_Notes
      ,LegalEntitlementL2L3_FundingAvailable
      ,LegalEntitlementL2L3_FundingApprovalStartDate
      ,LegalEntitlementL2L3_FundingApprovalEndDate
      ,LegalEntitlementL2L3_Notes
      ,LifelongLearningEntitlement_FundingAvailable
      ,LifelongLearningEntitlement_FundingApprovalStartDate
      ,LifelongLearningEntitlement_FundingApprovalEndDate
      ,LifelongLearningEntitlement_Notes
      ,LocalFlexibilities_FundingAvailable
      ,LocalFlexibilities_FundingApprovalStartDate
      ,LocalFlexibilities_FundingApprovalEndDate 
      ,LocalFlexibilities_Notes
from dbo.view_OutputChangedQualifications

Union

select Status
      ,DateOfOfqualDataSnapshot
      ,Title
      ,OrganisationName
      ,QualificationNumber
      ,Level
      ,QualificationType
      ,Subcategory
      ,SectorSubjectArea
      ,AdvancedLearnerLoans_FundingAvailable
      ,AdvancedLearnerLoans_FundingApprovalStartDate
      ,AdvancedLearnerLoans_FundingApprovalEndDate
      ,Age1416_FundingAvailable
      ,Age1416_FundingApprovalStartDate
      ,Age1416_FundingApprovalEndDate
      ,Age1416_Notes
      ,Age1619_FundingAvailable
      ,Age1619_FundingApprovalStartDate
      ,Age1619_FundingApprovalEndDate
      ,Age1619_Notes
      ,DigitalEntitlement_FundingAvailable
      ,DigitalEntitlement_FundingApprovalStartDate
      ,DigitalEntitlement_FundingApprovalEndDate
      ,DigitalEntitlement_Notes
      ,L3FreeCoursesForJobs_FundingAvailable
      ,L3FreeCoursesForJobs_FundingApprovalStartDate
      ,L3FreeCoursesForJobs_FundingApprovalEndDate
      ,L3FreeCoursesForJobs_Notes
      ,LegalEntitlementEnglishandMaths_FundingAvailable
      ,LegalEntitlementEnglishandMaths_FundingApprovalStartDate
      ,LegalEntitlementEnglishandMaths_FundingApprovalEndDate
      ,LegalEntitlementEnglishandMaths_Notes
      ,LegalEntitlementL2L3_FundingAvailable
      ,LegalEntitlementL2L3_FundingApprovalStartDate
      ,LegalEntitlementL2L3_FundingApprovalEndDate
      ,LegalEntitlementL2L3_Notes
      ,LifelongLearningEntitlement_FundingAvailable
      ,LifelongLearningEntitlement_FundingApprovalStartDate
      ,LifelongLearningEntitlement_FundingApprovalEndDate
      ,LifelongLearningEntitlement_Notes
      ,LocalFlexibilities_FundingAvailable
      ,LocalFlexibilities_FundingApprovalStartDate
      ,LocalFlexibilities_FundingApprovalEndDate 
      ,LocalFlexibilities_Notes
from dbo.view_OutputNewQualifications

union

Select Status
      ,DateOfOfqualDataSnapshot
      ,Title
      ,OrganisationName
      ,QualificationNumber
      ,Level
      ,QualificationType
      ,Subcategory
      ,SectorSubjectArea
      ,AdvancedLearnerLoans_FundingAvailable
      ,AdvancedLearnerLoans_FundingApprovalStartDate
      ,AdvancedLearnerLoans_FundingApprovalEndDate
      ,Age1416_FundingAvailable
      ,Age1416_FundingApprovalStartDate
      ,Age1416_FundingApprovalEndDate
      ,Age1416_Notes
      ,Age1619_FundingAvailable
      ,Age1619_FundingApprovalStartDate
      ,Age1619_FundingApprovalEndDate
      ,Age1619_Notes
      ,DigitalEntitlement_FundingAvailable
      ,DigitalEntitlement_FundingApprovalStartDate
      ,DigitalEntitlement_FundingApprovalEndDate
      ,DigitalEntitlement_Notes
      ,L3FreeCoursesForJobs_FundingAvailable
      ,L3FreeCoursesForJobs_FundingApprovalStartDate
      ,L3FreeCoursesForJobs_FundingApprovalEndDate
      ,L3FreeCoursesForJobs_Notes
      ,LegalEntitlementEnglishandMaths_FundingAvailable
      ,LegalEntitlementEnglishandMaths_FundingApprovalStartDate
      ,LegalEntitlementEnglishandMaths_FundingApprovalEndDate
      ,LegalEntitlementEnglishandMaths_Notes
      ,LegalEntitlementL2L3_FundingAvailable
      ,LegalEntitlementL2L3_FundingApprovalStartDate
      ,LegalEntitlementL2L3_FundingApprovalEndDate
      ,LegalEntitlementL2L3_Notes
      ,LifelongLearningEntitlement_FundingAvailable
      ,LifelongLearningEntitlement_FundingApprovalStartDate
      ,LifelongLearningEntitlement_FundingApprovalEndDate
      ,LifelongLearningEntitlement_Notes
      ,LocalFlexibilities_FundingAvailable
      ,LocalFlexibilities_FundingApprovalStartDate
      ,LocalFlexibilities_FundingApprovalEndDate 
      ,LocalFlexibilities_Notes
from dbo.view_OutputUnchangedQualifications
GO

