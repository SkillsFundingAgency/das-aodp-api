CREATE View [dbo].[v_Output_CombinedStatuses] as

/*##################################################################################################
	-Name:				Output Combined Statuses
	-Description:		View for combining all output views:
						- view_OutputChangedQualifications
						- view_OutputNewQualifications
						- view_OutputUnapprovedQualifications
						- view_OutputUnchangedQualifications
						
	-Date of Creation:	22/04/2025
	-Created By:		Adam Leaver
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		22/04/2025			Original
##################################################################################################*/

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
      ,Age1619_FundingAvailable
      ,Age1619_FundingApprovalStartDate
      ,Age1619_FundingApprovalEndDate
      ,DigitalEntitlement_FundingAvailable
      ,DigitalEntitlement_FundingApprovalStartDate
      ,DigitalEntitlement_FundingApprovalEndDate
      ,L3FreeCoursesForJobs_FundingAvailable
      ,L3FreeCoursesForJobs_FundingApprovalStartDate
      ,L3FreeCoursesForJobs_FundingApprovalEndDate
      ,LegalEntitlementEnglishandMaths_FundingAvailable
      ,LegalEntitlementEnglishandMaths_FundingApprovalStartDate
      ,LegalEntitlementEnglishandMaths_FundingApprovalEndDate
      ,LegalEntitlementL2L3_FundingAvailable
      ,LegalEntitlementL2L3_FundingApprovalStartDate
      ,LegalEntitlementL2L3_FundingApprovalEndDate
      ,LifelongLearningEntitlement_FundingAvailable
      ,LifelongLearningEntitlement_FundingApprovalStartDate
      ,LifelongLearningEntitlement_FundingApprovalEndDate
      ,LocalFlexibilities_FundingAvailable
      ,LocalFlexibilities_FundingApprovalStartDate
      ,LocalFlexibilities_FundingApprovalEndDate 
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
      ,Age1619_FundingAvailable
      ,Age1619_FundingApprovalStartDate
      ,Age1619_FundingApprovalEndDate
      ,DigitalEntitlement_FundingAvailable
      ,DigitalEntitlement_FundingApprovalStartDate
      ,DigitalEntitlement_FundingApprovalEndDate
      ,L3FreeCoursesForJobs_FundingAvailable
      ,L3FreeCoursesForJobs_FundingApprovalStartDate
      ,L3FreeCoursesForJobs_FundingApprovalEndDate
      ,LegalEntitlementEnglishandMaths_FundingAvailable
      ,LegalEntitlementEnglishandMaths_FundingApprovalStartDate
      ,LegalEntitlementEnglishandMaths_FundingApprovalEndDate
      ,LegalEntitlementL2L3_FundingAvailable
      ,LegalEntitlementL2L3_FundingApprovalStartDate
      ,LegalEntitlementL2L3_FundingApprovalEndDate
      ,LifelongLearningEntitlement_FundingAvailable
      ,LifelongLearningEntitlement_FundingApprovalStartDate
      ,LifelongLearningEntitlement_FundingApprovalEndDate
      ,LocalFlexibilities_FundingAvailable
      ,LocalFlexibilities_FundingApprovalStartDate
      ,LocalFlexibilities_FundingApprovalEndDate
from dbo.view_OutputNewQualifications

union

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
      ,Age1619_FundingAvailable
      ,Age1619_FundingApprovalStartDate
      ,Age1619_FundingApprovalEndDate
      ,DigitalEntitlement_FundingAvailable
      ,DigitalEntitlement_FundingApprovalStartDate
      ,DigitalEntitlement_FundingApprovalEndDate
      ,L3FreeCoursesForJobs_FundingAvailable
      ,L3FreeCoursesForJobs_FundingApprovalStartDate
      ,L3FreeCoursesForJobs_FundingApprovalEndDate
      ,LegalEntitlementEnglishandMaths_FundingAvailable
      ,LegalEntitlementEnglishandMaths_FundingApprovalStartDate
      ,LegalEntitlementEnglishandMaths_FundingApprovalEndDate
      ,LegalEntitlementL2L3_FundingAvailable
      ,LegalEntitlementL2L3_FundingApprovalStartDate
      ,LegalEntitlementL2L3_FundingApprovalEndDate
      ,LifelongLearningEntitlement_FundingAvailable
      ,LifelongLearningEntitlement_FundingApprovalStartDate
      ,LifelongLearningEntitlement_FundingApprovalEndDate
      ,LocalFlexibilities_FundingAvailable
      ,LocalFlexibilities_FundingApprovalStartDate
      ,LocalFlexibilities_FundingApprovalEndDate
from dbo.view_OutputUnapprovedQualifications

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
      ,Age1619_FundingAvailable
      ,Age1619_FundingApprovalStartDate
      ,Age1619_FundingApprovalEndDate
      ,DigitalEntitlement_FundingAvailable
      ,DigitalEntitlement_FundingApprovalStartDate
      ,DigitalEntitlement_FundingApprovalEndDate
      ,L3FreeCoursesForJobs_FundingAvailable
      ,L3FreeCoursesForJobs_FundingApprovalStartDate
      ,L3FreeCoursesForJobs_FundingApprovalEndDate
      ,LegalEntitlementEnglishandMaths_FundingAvailable
      ,LegalEntitlementEnglishandMaths_FundingApprovalStartDate
      ,LegalEntitlementEnglishandMaths_FundingApprovalEndDate
      ,LegalEntitlementL2L3_FundingAvailable
      ,LegalEntitlementL2L3_FundingApprovalStartDate
      ,LegalEntitlementL2L3_FundingApprovalEndDate
      ,LifelongLearningEntitlement_FundingAvailable
      ,LifelongLearningEntitlement_FundingApprovalStartDate
      ,LifelongLearningEntitlement_FundingApprovalEndDate
      ,LocalFlexibilities_FundingAvailable
      ,LocalFlexibilities_FundingApprovalStartDate
      ,LocalFlexibilities_FundingApprovalEndDate
from dbo.view_OutputUnchangedQualifications
GO

