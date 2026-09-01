using Moq;
using Shouldly;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using SFA.DAS.AODP.Application.Services.FundingExtension;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Rollover;

public class GetRolloverStartSummaryQueryHandlerTests : UnitTest
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsSummary_ReturnsSuccessfulResponse()
    {
        var repository = new Mock<IRolloverRepository>();
        var academicYearService = new Mock<IAcademicYearService>();

        academicYearService.Setup(x => x.GetCurrentAcademicYear()).Returns("2024/25");

        var summary = new RolloverStartSummary
        {
            TotalCandidatesCount = 10,
            CandidatesEligibleCount = 4,
            CandidatesIneligibleCount = 3,
            CandidatesRemainingCount = 2
        };

        repository.Setup(x => x.GetRolloverStartSummaryAsync("2024/25", CancellationToken))
                  .ReturnsAsync(summary);

        var sut = new GetRolloverStartSummaryQueryHandler(repository.Object, academicYearService.Object);

        var result = await sut.Handle(new GetRolloverStartSummaryQuery(), CancellationToken);

        result.Success.ShouldBeTrue();
        result.Value.TotalCandidatesCount.ShouldBe(10);
        result.Value.CandidatesEligibleCount.ShouldBe(4);
        result.Value.CandidatesIneligibleCount.ShouldBe(3);
        result.Value.CandidatesRemainingCount.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ReturnsFailedResponse()
    {
        var repository = new Mock<IRolloverRepository>();
        var academicYearService = new Mock<IAcademicYearService>();

        academicYearService.Setup(x => x.GetCurrentAcademicYear()).Returns("2024/25");

        repository.Setup(x => x.GetRolloverStartSummaryAsync("2024/25", CancellationToken))
                  .ReturnsAsync((RolloverStartSummary?)null);

        var sut = new GetRolloverStartSummaryQueryHandler(repository.Object, academicYearService.Object);

        var result = await sut.Handle(new GetRolloverStartSummaryQuery(), CancellationToken);

        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<GetRolloverStartSummaryQueryResponse>();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailedResponse()
    {
        var repository = new Mock<IRolloverRepository>();
        var academicYearService = new Mock<IAcademicYearService>();

        academicYearService.Setup(x => x.GetCurrentAcademicYear()).Returns("2024/25");

        var exception = new InvalidOperationException("boom");

        repository.Setup(x => x.GetRolloverStartSummaryAsync("2024/25", CancellationToken))
                  .ThrowsAsync(exception);

        var sut = new GetRolloverStartSummaryQueryHandler(repository.Object, academicYearService.Object);

        var result = await sut.Handle(new GetRolloverStartSummaryQuery(), CancellationToken);

        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("boom");
        result.InnerException.ShouldBeSameAs(exception);
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<GetRolloverStartSummaryQueryResponse>();
    }
}
