using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace SFA.DAS.AODP.Data.EntityConfiguration
{
    [ExcludeFromCodeCoverage]
    public class View_SectionPageCountForRoutingEntityConfiguration : IEntityTypeConfiguration<View_SectionPageCount>
    {
        public void Configure(EntityTypeBuilder<View_SectionPageCount> builder)
        {
            builder.HasKey(k => k.SectionId);
            builder.ToView("View_SectionPageCount");

        }
    }
}
