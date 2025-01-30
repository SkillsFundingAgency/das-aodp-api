using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.AODP.Data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.EntityConifguration
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
