using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration
{
    [ExcludeFromCodeCoverage]
    public class View_SectionSummaryForApplicationEntityConfiguration : IEntityTypeConfiguration<View_SectionSummaryForApplication>
    {
        public void Configure(EntityTypeBuilder<View_SectionSummaryForApplication> builder)
        {
            builder.HasKey(k => new { k.SectionId, k.ApplicationId });
            builder.ToView("View_SectionSummaryForApplication");

        }
    }
}
