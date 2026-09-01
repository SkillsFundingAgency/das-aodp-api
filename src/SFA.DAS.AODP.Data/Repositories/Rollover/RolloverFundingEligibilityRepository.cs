using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class RolloverFundingEligibilityRepository(
    IApplicationDbContext context,
    IAcademicYearProvider academicYearProvider)
    : IRolloverFundingEligibilityRepository
{
    public async Task<IReadOnlyCollection<RolloverFundingEligibility>> GetAsync(
        IReadOnlyCollection<FundingChangeKey> keys,
        CancellationToken cancellationToken)
    {
        var distinctKeys = keys.Distinct().ToList();
        var results = new List<RolloverFundingEligibility>(distinctKeys.Count);

        results.AddRange(await GetOfqualAsync(
            distinctKeys.Where(x => x.SourceType == RolloverSourceTypes.Ofqual).ToList(),
            cancellationToken));

        results.AddRange(await GetQaaAsync(
            distinctKeys.Where(x => x.SourceType == RolloverSourceTypes.Qaa).ToList(),
            cancellationToken));

        var unknownSource = distinctKeys.FirstOrDefault(x =>
            x.SourceType != RolloverSourceTypes.Ofqual &&
            x.SourceType != RolloverSourceTypes.Qaa);

        if (unknownSource is not null)
        {
            throw new NotSupportedException(
                $"Rollover funding source type '{unknownSource.SourceType}' is not supported.");
        }

        return results;
    }

    private async Task<IReadOnlyCollection<RolloverFundingEligibility>> GetOfqualAsync(
        IReadOnlyCollection<FundingChangeKey> keys,
        CancellationToken cancellationToken)
    {
        if (keys.Count == 0)
        {
            return [];
        }

        var sourceQualificationIds = keys.Select(x => x.SourceQualificationId).Distinct().ToList();
        var fundingOfferIds = keys.Select(x => x.FundingOfferId).Distinct().ToList();

        var fundings = await context.QualificationFundings
            .AsNoTracking()
            .Where(funding =>
                sourceQualificationIds.Contains(funding.QualificationVersionId) &&
                fundingOfferIds.Contains(funding.FundingOfferId))
            .Select(funding => new
                {
                    funding.QualificationVersionId,
                    funding.FundingOfferId,
                    funding.EndDate,
                    IsEligibleForFunding = funding.QualificationVersion.EligibleForFunding == true,
                    IsLatestVersion = !context.QualificationVersions.Any(other =>
                        other.QualificationId == funding.QualificationVersion.QualificationId &&
                        (
                            (other.Version ?? 0) > (funding.QualificationVersion.Version ?? 0) ||
                            (
                                (other.Version ?? 0) == (funding.QualificationVersion.Version ?? 0) &&
                                other.LastUpdatedDate > funding.QualificationVersion.LastUpdatedDate
                            ) ||
                            (
                                (other.Version ?? 0) == (funding.QualificationVersion.Version ?? 0) &&
                                other.LastUpdatedDate == funding.QualificationVersion.LastUpdatedDate &&
                                other.InsertedDate > funding.QualificationVersion.InsertedDate
                            )
                        ))
                })
            .ToListAsync(cancellationToken);

        return keys.Select(key =>
        {
            var academicYear = key.AcademicYear ?? academicYearProvider.GetCurrentAcademicYear();
            var (startDate, endDate) = GetAcademicYearDates(academicYear);
            var funding = fundings.FirstOrDefault(x =>
                x.QualificationVersionId == key.SourceQualificationId &&
                x.FundingOfferId == key.FundingOfferId);

            var isEligible = funding is not null &&
                             funding.IsEligibleForFunding &&
                             funding.IsLatestVersion &&
                             IsActiveForAcademicYear(funding.EndDate, startDate, endDate);

            return new RolloverFundingEligibility(
                key,
                academicYear,
                funding?.EndDate,
                isEligible);
        }).ToList();
    }

    private async Task<IReadOnlyCollection<RolloverFundingEligibility>> GetQaaAsync(
        IReadOnlyCollection<FundingChangeKey> keys,
        CancellationToken cancellationToken)
    {
        if (keys.Count == 0)
        {
            return [];
        }

        var sourceQualificationIds = keys.Select(x => x.SourceQualificationId).Distinct().ToList();
        var fundingOfferIds = keys.Select(x => x.FundingOfferId).Distinct().ToList();
        var fundings = await context.QaaQualificationFundings
            .AsNoTracking()
            .Where(x =>
                sourceQualificationIds.Contains(x.QaaQualificationId) &&
                fundingOfferIds.Contains(x.FundingOfferId))
            .Select(x => new
            {
                x.QaaQualificationId,
                x.FundingOfferId,
                x.EndDate
            })
            .ToListAsync(cancellationToken);

        return keys.Select(key =>
        {
            var academicYear = key.AcademicYear ?? academicYearProvider.GetCurrentAcademicYear();
            var (startDate, endDate) = GetAcademicYearDates(academicYear);
            var funding = fundings.FirstOrDefault(x =>
                x.QaaQualificationId == key.SourceQualificationId &&
                x.FundingOfferId == key.FundingOfferId);

            return new RolloverFundingEligibility(
                key,
                academicYear,
                funding?.EndDate,
                funding is not null &&
                IsActiveForAcademicYear(funding.EndDate, startDate, endDate));
        }).ToList();
    }

    private static bool IsActiveForAcademicYear(
        DateOnly? fundingEndDate,
        DateOnly academicYearStart,
        DateOnly academicYearEnd)
    {
        return fundingEndDate is null ||
               fundingEndDate >= academicYearStart &&
               fundingEndDate <= academicYearEnd;
    }

    private static (DateOnly StartDate, DateOnly EndDate) GetAcademicYearDates(string academicYear)
    {
        var parts = academicYear.Split('/');
        if (parts.Length != 2 ||
            !int.TryParse(parts[0], out var startYear) ||
            !int.TryParse(parts[1], out var shortEndYear))
        {
            throw new ArgumentException(
                "Academic year must be in the format 'YYYY/YY'.",
                nameof(academicYear));
        }

        var endYear = startYear / 100 * 100 + shortEndYear;
        if (endYear <= startYear)
        {
            endYear += 100;
        }

        return (new DateOnly(startYear, 8, 1), new DateOnly(endYear, 7, 31));
    }
}
