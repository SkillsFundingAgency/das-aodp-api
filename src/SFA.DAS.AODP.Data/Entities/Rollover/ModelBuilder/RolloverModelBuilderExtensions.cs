using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.AODP.Data.Entities.Rollover.ModelBuilder;

/// <summary>
/// Provides extension methods for configuring the rollover-related entities in the Entity Framework Core model builder.
/// </summary>
[ExcludeFromCodeCoverage]
public static class RolloverModelBuilderExtensions
{
    /// <summary>
    /// Adds in rollover related model builder configurations, including property conversions and relationships between entities.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    /// <returns>The updated model builder.</returns>
    public static Microsoft.EntityFrameworkCore.ModelBuilder AddRollover(this Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolloverCandidates>(b =>
        {
            b.Property(x => x.RolloverStatus)
                .HasConversion<string>();

            b.HasOne(x => x.DecisionRun)
                .WithMany()
                .HasForeignKey(x => x.RolloverDecisionRunId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<RolloverWorkflowRun>(b =>
        {
            b.Property(x => x.SelectionMethod)
                .HasConversion<string>();

            b.Navigation(x => x.Candidates).HasField("_candidates");
            b.Navigation(x => x.FundingOffers).HasField("_fundingOffers");
            b.Navigation(x => x.Filters).HasField("_filters");
        });

        modelBuilder.Entity<RolloverWorkflowRunFundingOffer>(b =>
        {
            b.HasOne(x => x.WorkflowRun)
                .WithMany(r => r.FundingOffers)
                .HasForeignKey(x => x.RolloverWorkflowRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolloverWorkflowRunFilter>(b =>
        {
            b.Property(x => x.FilterKey)
                .HasConversion<string>();

            b.HasOne(x => x.WorkflowRun)
                .WithMany(r => r.Filters)
                .HasForeignKey(x => x.RolloverWorkflowRunId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Values).HasField("_values");
        });

        modelBuilder.Entity<RolloverWorkflowRunFilterValue>(b =>
        {
            b.HasOne(x => x.Filter)
                .WithMany(f => f.Values)
                .HasForeignKey(x => x.RolloverWorkflowRunFilterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolloverWorkflowCandidate>(b =>
        {
            b.HasOne(x => x.WorkflowRun)
                .WithMany(r => r.Candidates)
                .HasForeignKey(x => x.RolloverWorkflowRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolloverDecisionRun>(b =>
        {
            b.Property(x => x.SelectionMethod)
                .HasConversion<string>();
        });

        return modelBuilder;
    }
}