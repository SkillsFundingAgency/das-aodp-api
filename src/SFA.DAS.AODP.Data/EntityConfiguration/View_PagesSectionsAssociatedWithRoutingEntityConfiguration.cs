using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration
{
    [ExcludeFromCodeCoverage]
    public class View_PagesSectionsAssociatedWithRoutingEntityConfiguration : IEntityTypeConfiguration<View_PagesSectionsAssociatedWithRouting>
    {
        public void Configure(EntityTypeBuilder<View_PagesSectionsAssociatedWithRouting> builder)
        {
            builder.HasNoKey();
            builder.ToView("View_PagesSectionsAssociatedWithRouting");
        }
    }
}