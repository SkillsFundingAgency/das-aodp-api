using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.ValueConverters;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration;

[ExcludeFromCodeCoverage]
public class AllQualificationFundingEntityConfiguration : IEntityTypeConfiguration<AllQualificationFunding>
{
    public void Configure(EntityTypeBuilder<AllQualificationFunding> builder)
    {
        builder
            .ToView("AllQualificationFundings", "funded")
            .HasNoKey();

        builder
            .Property(q => q.FundingApprovalStartDate)
            .HasConversion<NullableDateOnlyToDateTimeConverter>();

        builder
            .Property(q => q.FundingApprovalEndDate)
            .HasConversion<NullableDateOnlyToDateTimeConverter>();
    }
}
