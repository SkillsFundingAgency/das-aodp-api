using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Testing.Testing;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class PldnsTests : UnitTest
{
    [Fact]
    public void Pldns_ForFundingStream_LegalEntitlementEnglishAndMaths()
    {
        // Arrange
        var pldns = CreatePldns();

        // Act
        var result = pldns.ForFundingStream(FundingStream.LegalEntitlementEnglishAndMaths);

        result.ShouldBe(new DateTime(2025, 04, 01));
    }

    [Fact]
    public void Pldns_ForFundingStream_LegalEntitlementL2L3()
    {
        // Arrange
        var pldns = CreatePldns();

        // Act
        var result = pldns.ForFundingStream(FundingStream.LegalEntitlementL2L3);

        // Assert
        result.ShouldBe(new DateTime(2025, 05, 01));
    }

    [Fact]
    public void Pldns_ForFundingStream_Level3FCoursesForJobs()
    {
        var pldns = CreatePldns();

        var result = pldns.ForFundingStream(FundingStream.FreeCoursesForJobs);

        result.ShouldBe(new DateTime(2025, 06, 01));
    }

    [Fact]
    public void Pldns_ForFundingStream_LifelongLearning()
    {
        var pldns = CreatePldns();

        var result = pldns.ForFundingStream(FundingStream.LifelongLearningEntitlement);

        result.ShouldBe(new DateTime(2025, 07, 01));
    }

    [Fact]
    public void Pldns_ForFundingStream_Pldns16To19()
    {
        var pldns = CreatePldns();

        var result = pldns.ForFundingStream(FundingStream.Age1619);

        result.ShouldBe(new DateTime(2025, 08, 01));
    }

    [Fact]
    public void Pldns_ForFundingStream_Pldns14To16()
    {
        var pldns = CreatePldns();

        var result = pldns.ForFundingStream(FundingStream.Age1416);

        result.ShouldBe(new DateTime(2025, 09, 01));
    }

    private Pldns CreatePldns() =>
        new()
        {
            DigitalEntitlement = new DateTime(2025, 01, 01),
            Cof = new DateTime(2025, 02, 01),
            EsfL3L4 = new DateTime(2025, 03, 01),
            LegalEntitlementEngMaths = new DateTime(2025, 04, 01),
            LegalEntitlementL2L3 = new DateTime(2025, 05, 01),
            Level3FCoursesForJobs = new DateTime(2025, 06, 01),
            LifelongLearning = new DateTime(2025, 07, 01),
            Pldns16To19 = new DateTime(2025, 08, 01),
            Pldns14To16 = new DateTime(2025, 09, 01)
        };
}