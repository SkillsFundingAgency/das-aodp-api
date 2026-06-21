using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.ValueConverters;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration;

[ExcludeFromCodeCoverage]
public class QaaEntityConfiguration : IEntityTypeConfiguration<RegulatedQaaQualification>
{
    public void Configure(EntityTypeBuilder<RegulatedQaaQualification> builder)
    {
        builder
            .Property(q => q.StartDate)
            .HasConversion<DateOnlyToDateTimeConverter>()
            .HasColumnType("datetime2");

        builder
            .Property(q => q.LastDateForRegistration)
            .HasConversion<DateOnlyToDateTimeConverter>()
            .HasColumnType("datetime2");

        builder
            .Property(q => q.Age1619FundingApprovalEndDate)
            .HasConversion<NullableDateOnlyToDateTimeConverter>()
            .HasColumnType("datetime2");

        builder
            .Property(q => q.AdvancedLearnerLoansFundingApprovalEndDate)
            .HasConversion<NullableDateOnlyToDateTimeConverter>()
            .HasColumnType("datetime2");

        builder
            .Property(q => q.LegalEntitlementL2L3FundingApprovalEndDate)
            .HasConversion<NullableDateOnlyToDateTimeConverter>()
            .HasColumnType("datetime2");

        builder
            .Property(q => q.SectorSubjectArea)
            .HasConversion(
                ssaTier => ssaTier.Name,
                ssaName => SectorSubjectArea.FromName(ssaName));

        builder
            .Property(q => q.LatestImportComparisonOutcome)
            .HasConversion<string>()
            .HasColumnType("nvarchar(50)");

        builder
            .Property(q => q.LastDateForRegistrationChangeType)
            .HasConversion<string>()
            .HasColumnType("nvarchar(50)");
    }
}