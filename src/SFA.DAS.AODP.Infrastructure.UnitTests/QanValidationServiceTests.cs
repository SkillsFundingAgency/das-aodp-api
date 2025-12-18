using Moq;
using RestEase;
using SFA.DAS.AODP.Infrastructure.Services;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Qualification;
using SFA.DAS.AODP.Models.Validation;
using System.Net;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Application;

public class QanValidationServiceTests
{
    private readonly Mock<IQualificationsApi> _qualificationsApi = new();
    private readonly IQanValidationService _service;

    private const string Qan = "1234567";
    private const string Title_Level3Engineering = "Level 3 Diploma in Engineering";
    private const string Title_Level3Mechanics = "Level 3 Diploma in Mechanics";
    private const string AO_CityAndGuilds = "City & Guilds";
    private const string AO_Pearson = "Pearson";
    private const string ApiUrl = "http://localhost/qualifications";

    public QanValidationServiceTests()
    {
        _service = new QanValidationService(_qualificationsApi.Object);
    }

    [Theory]
    [MemberData(nameof(QanValidationCases))]
    public async Task ValidateAsync_QualificationFound_ReturnsExpectedValidationResult(
        string titleFromApi,
        string titleFromRequest,
        string organisationFromApi,
        string organisationFromRequest,
        bool isValid,
        string? validationMessage)
    {
        var qualification = new QualificationDTO
        {
            Title = titleFromApi,
            OrganisationName = organisationFromApi
        };

        _qualificationsApi
            .Setup(api => api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()))
            .ReturnsAsync(qualification);

        var result = await _service.ValidateAsync(
            Qan,
            titleFromRequest,
            organisationFromRequest,
            default);

        Assert.Multiple(() =>
        {
            Assert.Equal(isValid, result.IsValid);
            Assert.Equal(validationMessage, result.ValidationMessage);

            _qualificationsApi.Verify(api =>
                api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task ValidateAsync_QualificationNotFound_ReturnsInvalidResult()
    {
        _qualificationsApi
            .Setup(api => api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<QualificationDTO?>(null));

        var result = await _service.ValidateAsync(
            Qan,
            Title_Level3Engineering,
            AO_Pearson,
            default);

        var expectedMessage = string.Format(QanValidationMessages.QualificationNotFound, Qan);

        Assert.Multiple(() =>
        {
            Assert.False(result.IsValid);
            Assert.Equal(expectedMessage, result.ValidationMessage);

            _qualificationsApi.Verify(api =>
                api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task ValidateAsync_ApiThrowsNotFoundApiException_ReturnsInvalidResult()
    {
        var apiException = CreateApiException(HttpStatusCode.NotFound);

        _qualificationsApi
            .Setup(api => api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var result = await _service.ValidateAsync(
            Qan,
            Title_Level3Engineering,
            AO_Pearson,
            default);

        var expectedMessage = string.Format(QanValidationMessages.QualificationNotFound, Qan);

        Assert.Multiple(() =>
        {
            Assert.False(result.IsValid);
            Assert.Equal(expectedMessage, result.ValidationMessage);

            _qualificationsApi.Verify(api =>
                api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task ValidateAsync_ApiThrowsNonNotFoundApiException_PropagatesException()
    {
        var apiException = CreateApiException(HttpStatusCode.InternalServerError);

        _qualificationsApi
            .Setup(api => api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            _service.ValidateAsync(Qan, Title_Level3Engineering, AO_Pearson, default));

        Assert.Multiple(() =>
        {
            Assert.IsType<ApiException>(exception);
            Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);

            _qualificationsApi.Verify(api =>
                api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    [Fact]
    public async Task ValidateAsync_ApiThrowsException_PropagatesException()
    {
        const string ExceptionMessage = "API failure";

        _qualificationsApi
            .Setup(api => api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(ExceptionMessage));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.ValidateAsync(Qan, Title_Level3Engineering, AO_Pearson, default));

        Assert.Multiple(() =>
        {
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal(ExceptionMessage, exception.Message);

            _qualificationsApi.Verify(api =>
                api.GetByQanAsync(Qan, It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    private static ApiException CreateApiException(HttpStatusCode statusCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl);
        var response = new HttpResponseMessage(statusCode);
        return new ApiException(request, response, string.Empty);
    }

    public static IEnumerable<object[]> QanValidationCases =>
        new[]
        {
            new object[]
            {
                Title_Level3Engineering,
                Title_Level3Mechanics,
                AO_Pearson,
                AO_Pearson,
                false,
                QanValidationMessages.TitleMismatch
            },
            new object[]
            {
                Title_Level3Engineering,
                Title_Level3Engineering,
                AO_Pearson,
                AO_CityAndGuilds,
                false,
                QanValidationMessages.OrganisationMismatch
            },
            new object[]
            {
                Title_Level3Engineering,
                Title_Level3Mechanics,
                AO_CityAndGuilds,
                AO_Pearson,
                false,
                QanValidationMessages.TitleAndOrganisationMismatch
            },
            new object[]
            {
                Title_Level3Engineering,
                Title_Level3Engineering,
                AO_CityAndGuilds,
                AO_CityAndGuilds,
                true,
                null
            }
        };
}
