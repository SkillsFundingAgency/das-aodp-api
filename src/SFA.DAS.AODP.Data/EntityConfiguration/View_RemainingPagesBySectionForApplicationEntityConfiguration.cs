using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.Application;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration
{
    [ExcludeFromCodeCoverage]
    public class View_RemainingPagesBySectionForApplicationEntityConfiguration : IEntityTypeConfiguration<View_RemainingPagesBySectionForApplication>
    {
        public void Configure(EntityTypeBuilder<View_RemainingPagesBySectionForApplication> builder)
        {
            builder.HasKey(k => new { k.SectionId, k.ApplicationId });
            builder.ToView("View_RemainingPagesBySectionForApplication");


            builder
           .HasOne(x => x.Application)
           .WithMany()
           .HasForeignKey(e => e.ApplicationId);
        }
    }
}
