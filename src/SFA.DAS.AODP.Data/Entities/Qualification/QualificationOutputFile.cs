namespace SFA.DAS.AODP.Data.Entities.Qualification
{
    public class QualificationOutputFile
    {
        public DateTime? DateOfOfqualDataSnapshot { get; set; }
        public string? AwardingOrganisation { get; set; }

        public string? QualificationName { get; set; }
        public string? QualificationNumber { get; set; }
        public string? Level { get; set; }
        public string? QualificationType { get; set; }
        public string? Subcategory { get; set; }
        public string? SectorSubjectArea { get; set; }
        public string? Status { get; set; }

        public bool? Age1416_FundingAvailable { get; set; }
        public DateTime? Age1416_FundingApprovalStartDate { get; set; }
        public DateTime? Age1416_FundingApprovalEndDate { get; set; }
        public string? Age1416_Notes { get; set; }

        public bool? Age1619_FundingAvailable { get; set; }
        public DateTime? Age1619_FundingApprovalStartDate { get; set; }
        public DateTime? Age1619_FundingApprovalEndDate { get; set; }
        public string? Age1619_Notes { get; set; }

        public bool? LocalFlexibilities_FundingAvailable { get; set; }
        public DateTime? LocalFlexibilities_FundingApprovalStartDate { get; set; }
        public DateTime? LocalFlexibilities_FundingApprovalEndDate { get; set; }
        public string? LocalFlexibilities_Notes { get; set; }

        public bool? LegalEntitlementEnglishandMaths_FundingAvailable { get; set; }
        public DateTime? LegalEntitlementEnglishandMaths_FundingApprovalStartDate { get; set; }
        public DateTime? LegalEntitlementEnglishandMaths_FundingApprovalEndDate { get; set; }
        public string? LegalEntitlementEnglishandMaths_Notes { get; set; }

        public bool? LegalEntitlementL2L3_FundingAvailable { get; set; }
        public DateTime? LegalEntitlementL2L3_FundingApprovalStartDate { get; set; }
        public DateTime? LegalEntitlementL2L3_FundingApprovalEndDate { get; set; }
        public string? LegalEntitlementL2L3_Notes { get; set; }


        public bool? DigitalEntitlement_FundingAvailable { get; set; }
        public DateTime? DigitalEntitlement_FundingApprovalStartDate { get; set; }
        public DateTime? DigitalEntitlement_FundingApprovalEndDate { get; set; }
        public string? DigitalEntitlement_Notes { get; set; }

        public bool? LifelongLearningEntitlement_FundingAvailable { get; set; }
        public DateTime? LifelongLearningEntitlement_FundingApprovalStartDate { get; set; }
        public DateTime? LifelongLearningEntitlement_FundingApprovalEndDate { get; set; }
        public string? LifelongLearningEntitlement_Notes { get; set; }

        public bool? AdvancedLearnerLoans_FundingAvailable { get; set; }
        public DateTime? AdvancedLearnerLoans_FundingApprovalStartDate { get; set; }
        public DateTime? AdvancedLearnerLoans_FundingApprovalEndDate { get; set; }
        public string? AdvancedLearnerLoans_Notes { get; set; }

        public bool? FreeCoursesForJobs_FundingAvailable { get; set; }
        public DateTime? FreeCoursesForJobs_FundingApprovalStartDate { get; set; }
        public DateTime? FreeCoursesForJobs_FundingApprovalEndDate { get; set; }
        public string? FreeCoursesForJobs_Notes { get; set; }
        
        public string? AwardingOrganisationURL { get; set; }

    }

}
