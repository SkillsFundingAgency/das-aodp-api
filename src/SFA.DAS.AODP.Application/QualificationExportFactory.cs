using System;
using System.Collections.Generic;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;

public static class QualificationExportFactory
{
    private static readonly Random _rand = new();

    public static IEnumerable<QualificationExport> CreateDummyQualifications(int count = 10)
    {
        var list = new List<QualificationExport>();
        var today = DateTime.Today;

        for (int i = 1; i <= count; i++)
        {
            bool isArchived = _rand.NextDouble() < 0.3; // ~30% archived/expired
            bool isActive = !isArchived;

            // Create start/end date pairs depending on state
            DateTime startDate;
            DateTime endDate;

            if (isActive)
            {
                startDate = today.AddDays(-_rand.Next(30, 365));    // started sometime in the past year
                endDate = today.AddDays(_rand.Next(1, 180));        // ends sometime in next 6 months
            }
            else
            {
                startDate = today.AddDays(-_rand.Next(365, 1000));  // started 1–3 years ago
                endDate = today.AddDays(-_rand.Next(1, 180));       // ended within last 6 months
            }

            var qual = new QualificationExport
            {
                DateOfOfqualDataSnapshot = today.AddDays(-_rand.Next(0, 60)),
                AwardingOrganisation = $"Organisation {i}",
                QualificationName = $"Qualification {i}",
                QualificationNumber = $"QAN{i:0000}",
                Level = $"{_rand.Next(1, 8)}",
                QualificationType = _rand.Next(2) == 0 ? "Diploma" : "Certificate",
                Subcategory = $"Subcategory {_rand.Next(1, 5)}",
                SectorSubjectArea = $"Sector {_rand.Next(1, 10)}",
                Status = isArchived ? "Expired" : "Active",

                Age1416_FundingAvailable = _rand.Next(2) == 1,
                Age1416_FundingApprovalStartDate = startDate,
                Age1416_FundingApprovalEndDate = endDate,

                Age1619_FundingAvailable = _rand.Next(2) == 1,
                Age1619_FundingApprovalStartDate = startDate,
                Age1619_FundingApprovalEndDate = endDate,

                LocalFlexibilities_FundingAvailable = _rand.Next(2) == 1,
                LocalFlexibilities_FundingApprovalStartDate = startDate,
                LocalFlexibilities_FundingApprovalEndDate = endDate,

                LegalEntitlementEnglishandMaths_FundingAvailable = _rand.Next(2) == 1,
                LegalEntitlementEnglishandMaths_FundingApprovalStartDate = startDate,
                LegalEntitlementEnglishandMaths_FundingApprovalEndDate = endDate,

                LegalEntitlementL2L3_FundingAvailable = _rand.Next(2) == 1,
                LegalEntitlementL2L3_FundingApprovalStartDate = startDate,
                LegalEntitlementL2L3_FundingApprovalEndDate = endDate,

                DigitalEntitlement_FundingAvailable = _rand.Next(2) == 1,
                DigitalEntitlement_FundingApprovalStartDate = startDate,
                DigitalEntitlement_FundingApprovalEndDate = endDate,

                LifelongLearningEntitlement_FundingAvailable = _rand.Next(2) == 1,
                LifelongLearningEntitlement_FundingApprovalStartDate = startDate,
                LifelongLearningEntitlement_FundingApprovalEndDate = endDate,

                AdvancedLearnerLoans_FundingAvailable = _rand.Next(2) == 1,
                AdvancedLearnerLoans_FundingApprovalStartDate = startDate,
                AdvancedLearnerLoans_FundingApprovalEndDate = endDate,

                FreeCoursesForJobs_FundingAvailable = _rand.Next(2) == 1,
                FreeCoursesForJobs_FundingApprovalStartDate = startDate,
                FreeCoursesForJobs_FundingApprovalEndDate = endDate,

                Archived = isArchived
            };

            list.Add(qual);
        }

        return list;
    }
}
