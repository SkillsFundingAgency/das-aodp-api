using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConfiguration
{

    [ExcludeFromCodeCoverage]
    public class View_AvailableQuestionsForRoutingEntityConfiguration : IEntityTypeConfiguration<View_AvailableQuestionsForRouting>
    {
        public void Configure(EntityTypeBuilder<View_AvailableQuestionsForRouting> builder)
        {
            builder.HasKey(k => k.QuestionId);
            builder.ToView("View_AvailableQuestionsForRouting");
        }
    }
}
