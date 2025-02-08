using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration
{
    [ExcludeFromCodeCoverage]
    public class View_QuestionRoutingDetailsEntityConfiguration : IEntityTypeConfiguration<View_QuestionRoutingDetail>
    {
        public void Configure(EntityTypeBuilder<View_QuestionRoutingDetail> builder)
        {
            builder.HasNoKey();
            builder.ToView("View_QuestionRoutingDetails");
        }
    }
}
