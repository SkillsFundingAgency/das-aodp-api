using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.Application;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConifguration
{
    [ExcludeFromCodeCoverage]
    public class ApplicationEntityConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
           // builder
           //.HasMany(x => x.View_RemainingPagesBySectionForApplications)
           //.WithOne()
           //.HasForeignKey(e => e.ApplicationId);
        }
    }
}
