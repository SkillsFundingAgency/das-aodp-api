using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.ValueConverters;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration;

[ExcludeFromCodeCoverage]
public class QaaEntityConfiguration : IEntityTypeConfiguration<RegulatedQaaQualification>
{
    public void Configure(EntityTypeBuilder<RegulatedQaaQualification> builder)
    {
        const string nvarchar50 = "nvarchar(50)";

        builder
            .Property(q => q.StartDate)
            .HasConversion<DateOnlyToDateTimeConverter>()
            .HasColumnType("date");

        builder
            .Property(q => q.LastDateForRegistration)
            .HasConversion<DateOnlyToDateTimeConverter>()
            .HasColumnType("date");

        builder
            .Property(q => q.SectorSubjectAreaName)
            .HasColumnName("SectorSubjectArea");

        builder
            .Property(q => q.LatestImportComparisonOutcome)
            .HasConversion<string>()
            .HasColumnType(nvarchar50);

        builder
            .Property(q => q.LastDateForRegistrationChangeType)
            .HasConversion<string>()
            .HasColumnType(nvarchar50);
    }
}
