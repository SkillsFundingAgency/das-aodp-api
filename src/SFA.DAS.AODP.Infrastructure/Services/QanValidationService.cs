using RestEase;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Qualification;
using SFA.DAS.AODP.Models.Validation;
using System.Net;

namespace SFA.DAS.AODP.Infrastructure.Services;

public class QanValidationService : IQanValidationService
{
    private readonly IQualificationsApi _qualificationsApi;

    public QanValidationService(IQualificationsApi qualificationsApi)
    {
        _qualificationsApi = qualificationsApi;
    }

    public async Task<QanValidationResult> ValidateAsync(
        string qan,
        string qualificationTitle,
        string organisationName,
        CancellationToken cancellationToken = default)
    {
        var result = new QanValidationResult();

        QualificationDTO? qualification;

        try
        {
            qualification = await _qualificationsApi.GetByQanAsync(qan, cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            qualification = null;
        }

        if (qualification == null)
        {
            result.IsValid = false;
            result.ValidationMessage =
                string.Format(QanValidationMessages.QualificationNotFound, qan);
            return result;
        }

        var titleMatches = string.Equals(
            qualification.Title?.Trim(),
            qualificationTitle?.Trim(),
            StringComparison.OrdinalIgnoreCase);

        var organisationMatches = string.Equals(
            qualification.OrganisationName?.Trim(),
            organisationName?.Trim(),
            StringComparison.OrdinalIgnoreCase);

        result.IsValid = titleMatches && organisationMatches;

        if (!result.IsValid)
        {
            result.ValidationMessage = BuildValidationMessage(titleMatches, organisationMatches);
        }

        return result;
    }

    private static string BuildValidationMessage(bool titleMatches, bool organisationMatches)
    {
        if (!titleMatches && !organisationMatches)
            return QanValidationMessages.TitleAndOrganisationMismatch;
        if (!titleMatches)
            return QanValidationMessages.TitleMismatch;
        return QanValidationMessages.OrganisationMismatch;
    }
}
