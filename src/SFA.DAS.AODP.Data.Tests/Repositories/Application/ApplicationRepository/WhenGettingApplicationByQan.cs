using Moq;
using Moq.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Tests.Repositories.Application.ApplicationRepository;
public class WhenGettingApplicationByQan
{
    private readonly Mock<IApplicationDbContext> _context = new();

    private readonly Data.Repositories.Application.ApplicationRepository _sut;

    public WhenGettingApplicationByQan() => _sut = new(_context.Object);

    [Theory]
    [InlineData("6038817Q", "6038817Q")] // exact match, no obliques on either side
    [InlineData("603/8817/Q", "6038817Q")] // application QAN entered with obliques
    [InlineData("6038817q", "6038817Q")] // application QAN entered in lower case
    [InlineData(" 6038817Q ", "6038817Q")] // application QAN with leading/trailing whitespace
    public async Task Then_Returns_Application_When_Qan_Matches_After_Normalisation(string storedQualificationNumber, string requestedQan)
    {
        // Arrange
        Entities.Application.Application application = new()
        {
            Id = Guid.NewGuid(),
            QualificationNumber = storedQualificationNumber,
            ApplicationReview = new Entities.Application.ApplicationReview
            {
                ApplicationReviewFeedbacks = new List<Entities.Application.ApplicationReviewFeedback>()
            }
        };

        var dbSet = new List<Entities.Application.Application>() { application };

        _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetByQan(requestedQan);

        // Assert
        Assert.Single(result);
        Assert.Equal(application.Id, result[0].Id);
    }

    [Fact]
    public async Task Then_Returns_Empty_When_Qan_Does_Not_Match()
    {
        // Arrange
        Entities.Application.Application application = new()
        {
            Id = Guid.NewGuid(),
            QualificationNumber = "603/8817/Q",
            ApplicationReview = new Entities.Application.ApplicationReview
            {
                ApplicationReviewFeedbacks = new List<Entities.Application.ApplicationReviewFeedback>()
            }
        };

        var dbSet = new List<Entities.Application.Application>() { application };

        _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetByQan("1112222X");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Then_Returns_Empty_When_Application_Has_No_Qan()
    {
        // Arrange
        Entities.Application.Application application = new()
        {
            Id = Guid.NewGuid(),
            QualificationNumber = null,
            ApplicationReview = new Entities.Application.ApplicationReview
            {
                ApplicationReviewFeedbacks = new List<Entities.Application.ApplicationReviewFeedback>()
            }
        };

        var dbSet = new List<Entities.Application.Application>() { application };

        _context.SetupGet(c => c.Applications).ReturnsDbSet(dbSet);

        // Act
        var result = await _sut.GetByQan("6038817Q");

        // Assert
        Assert.Empty(result);
    }
}
