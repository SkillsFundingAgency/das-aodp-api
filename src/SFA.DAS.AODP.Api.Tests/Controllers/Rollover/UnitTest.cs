namespace SFA.DAS.AODP.Api.UnitTests.Controllers.Rollover;

public class UnitTest
{
    public ITestContext CurrentContext => TestContext.Current;

    public CancellationToken CancellationToken => CurrentContext.CancellationToken;
}