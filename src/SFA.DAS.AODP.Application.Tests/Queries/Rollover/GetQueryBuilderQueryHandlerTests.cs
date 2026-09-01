using Moq;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Rollover;

public class GetLevelsForRolloverQueryBuilderQueryHandlerTests : UnitTest
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsLevels_ReturnsSuccessfulResponse()
    {
        // Arrange
        RolloverQueryBuilderLevel[] levels = [new() { Id = 1, Name = "Level 1" }];
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetAllLevelsForRolloverQueryBuilderAsync(CancellationToken)).ReturnsAsync(levels);
        var sut = new GetLevelsForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetLevelsForRolloverQueryBuilderQuery(), CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Value.Levels.ShouldBeSameAs(levels);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailedResponse()
    {
        // Arrange
        var exception = new InvalidOperationException("Unable to load levels.");
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetAllLevelsForRolloverQueryBuilderAsync(CancellationToken)).ThrowsAsync(exception);
        var sut = new GetLevelsForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetLevelsForRolloverQueryBuilderQuery(), CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe(exception.Message);
        result.InnerException.ShouldBeSameAs(exception);
    }
}

public class GetTypesForRolloverQueryBuilderQueryHandlerTests : UnitTest
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsTypes_ReturnsSuccessfulResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderTypesRequest([1]);
        RolloverQueryBuilderType[] types = [new() { Id = 2, Name = "Type" }];
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetTypesForRolloverQueryBuilderAsync(filters, CancellationToken)).ReturnsAsync(types);
        var sut = new GetTypesForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetTypesForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Value.Types.ShouldBeSameAs(types);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailedResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderTypesRequest([1]);
        var exception = new InvalidOperationException("Unable to load types.");
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetTypesForRolloverQueryBuilderAsync(filters, CancellationToken)).ThrowsAsync(exception);
        var sut = new GetTypesForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetTypesForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe(exception.Message);
        result.InnerException.ShouldBeSameAs(exception);
    }
}

public class GetSectorSubjectAreasForRolloverQueryBuilderQueryHandlerTests : UnitTest
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsSectorSubjectAreas_ReturnsSuccessfulResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderSectorSubjectAreaRequest([1], [2]);
        RolloverQueryBuilderSectorSubjectArea[] sectorSubjectAreas = [new() { Id = "1.1", Name = "Medicine" }];
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetSectorSubjectAreasForRolloverQueryBuilderAsync(filters, CancellationToken)).ReturnsAsync(sectorSubjectAreas);
        var sut = new GetSectorSubjectAreasForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetSectorSubjectAreasForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Value.SectorSubjectAreas.ShouldBeSameAs(sectorSubjectAreas);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailedResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderSectorSubjectAreaRequest([1], [2]);
        var exception = new InvalidOperationException("Unable to load sector subject areas.");
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetSectorSubjectAreasForRolloverQueryBuilderAsync(filters, CancellationToken)).ThrowsAsync(exception);
        var sut = new GetSectorSubjectAreasForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetSectorSubjectAreasForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe(exception.Message);
        result.InnerException.ShouldBeSameAs(exception);
    }
}

public class GetAwardingOrganisationsForRolloverQueryBuilderQueryHandlerTests : UnitTest
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsAwardingOrganisations_ReturnsSuccessfulResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderAwardingOrganisationsRequest([1], [2], ["1.1"]);
        RolloverQueryBuilderAwardingOrganisation[] organisations = [new() { Id = Guid.NewGuid(), NameLegal = "Organisation" }];
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetAwardingOrganisationsForRolloverQueryBuilderAsync(filters, CancellationToken)).ReturnsAsync(organisations);
        var sut = new GetAwardingOrganisationsForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetAwardingOrganisationsForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Value.AwardingOrganisations.ShouldBeSameAs(organisations);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailedResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderAwardingOrganisationsRequest([1], [2], ["1.1"]);
        var exception = new InvalidOperationException("Unable to load awarding organisations.");
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetAwardingOrganisationsForRolloverQueryBuilderAsync(filters, CancellationToken)).ThrowsAsync(exception);
        var sut = new GetAwardingOrganisationsForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetAwardingOrganisationsForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe(exception.Message);
        result.InnerException.ShouldBeSameAs(exception);
    }
}

public class GetQualificationVersionsForRolloverQueryBuilderQueryHandlerTests : UnitTest
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsQualificationVersions_ReturnsSuccessfulResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderRequest { LevelIds = [1], TypeIds = [2] };
        RolloverCandidateDto[] qualifications = [new() { Id = Guid.NewGuid(), QualificationNumber = "QAN1" }];
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetQualificationVersionsForRolloverQueryBuilderAsync(filters, CancellationToken)).ReturnsAsync(qualifications);
        var sut = new GetQualificationVersionsForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetQualificationVersionsForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Value.QualificationVersions.ShouldBeSameAs(qualifications);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailedResponse()
    {
        // Arrange
        var filters = new RolloverQueryBuilderRequest { LevelIds = [1], TypeIds = [2] };
        var exception = new InvalidOperationException("Unable to load qualification versions.");
        var repository = new Mock<IRolloverRepository>();
        repository.Setup(x => x.GetQualificationVersionsForRolloverQueryBuilderAsync(filters, CancellationToken)).ThrowsAsync(exception);
        var sut = new GetQualificationVersionsForRolloverQueryBuilderQueryHandler(repository.Object);

        // Act
        var result = await sut.Handle(new GetQualificationVersionsForRolloverQueryBuilderQuery(filters), CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe(exception.Message);
        result.InnerException.ShouldBeSameAs(exception);
    }
}
