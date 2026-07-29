using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.ValueConverters;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration;

[ExcludeFromCodeCoverage]
public class QaaQualificationFundingEntityConfiguration : IEntityTypeConfiguration<QaaQualificationFunding>
{
    public void Configure(EntityTypeBuilder<QaaQualificationFunding> builder)
    {
        builder.HasKey(q => q.Id);

        builder
            .Property(q => q.StartDate)
            .HasConversion<NullableDateOnlyToDateTimeConverter>()
            .HasColumnType("date");

        builder
            .Property(q => q.EndDate)
            .HasConversion<NullableDateOnlyToDateTimeConverter>()
            .HasColumnType("date");

        builder
            .Property(q => q.FundingStatus)
            .HasMaxLength(255);

        builder
            .HasOne(q => q.QaaQualification)
            .WithMany(q => q.Fundings)
            .HasForeignKey(q => q.QaaQualificationId);

        builder
            .HasOne(q => q.FundingOffer)
            .WithMany()
            .HasForeignKey(q => q.FundingOfferId);

        builder
            .HasIndex(q => q.QaaQualificationId)
            .HasDatabaseName("IX_QaaQualificationFundings_QaaQualification");

        builder
            .HasIndex(q => new { q.QaaQualificationId, q.FundingOfferId })
            .IsUnique()
            .HasDatabaseName("UX_QaaQualificationFundings_Qualification_Offer");
    }
}
