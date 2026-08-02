using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class RolloverFundingUpdateRepository : IRolloverFundingUpdateRepository
{
    private readonly IApplicationDbContext _context;

    public RolloverFundingUpdateRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RolloverFundingUpdate>> GetFundingUpdatesAsync(
        List<SourceQualificationFundingKey> candidates,
        CancellationToken cancellationToken)
    {
        var updates = new List<RolloverFundingUpdate>();

        updates.AddRange(await GetOfqualFundingUpdatesAsync(candidates, cancellationToken));
        updates.AddRange(await GetQaaFundingUpdatesAsync(candidates, cancellationToken));

        return updates;
    }

    private async Task<List<RolloverFundingUpdate>> GetOfqualFundingUpdatesAsync(
        List<SourceQualificationFundingKey> candidates,
        CancellationToken cancellationToken)
    {
        var ofqualCandidates = candidates
            .Where(x => x.SourceType == RolloverSourceTypes.Ofqual)
            .ToList();
        var sourceQualificationIds = ofqualCandidates
            .Select(x => x.SourceQualificationId)
            .Distinct()
            .ToList();
        var fundingOfferIds = ofqualCandidates
            .Select(x => x.FundingOfferId)
            .Distinct()
            .ToList();

        if (ofqualCandidates.Count == 0)
        {
            return [];
        }

        var fundings = await _context.QualificationFundings
            .Where(qf =>
                sourceQualificationIds.Contains(qf.QualificationVersionId) &&
                fundingOfferIds.Contains(qf.FundingOfferId))
            .ToListAsync(cancellationToken);

        return ofqualCandidates
            .Join(
                fundings,
                key => new { key.SourceQualificationId, key.FundingOfferId },
                funding => new
                {
                    SourceQualificationId = funding.QualificationVersionId,
                    funding.FundingOfferId
                },
                (key, funding) => new RolloverFundingUpdate(
                    RolloverSourceTypes.Ofqual,
                    funding.QualificationVersionId,
                    funding.FundingOfferId,
                    key.AcademicYear,
                    funding.EndDate,
                    (endDate, comments, _) => funding.UpdateFunding(
                        funding.StartDate,
                        endDate,
                        comments)))
            .ToList();
    }

    private async Task<List<RolloverFundingUpdate>> GetQaaFundingUpdatesAsync(
        List<SourceQualificationFundingKey> candidates,
        CancellationToken cancellationToken)
    {
        var qaaCandidates = candidates
            .Where(x => x.SourceType == RolloverSourceTypes.Qaa)
            .ToList();
        var sourceQualificationIds = qaaCandidates
            .Select(x => x.SourceQualificationId)
            .Distinct()
            .ToList();
        var fundingOfferIds = qaaCandidates
            .Select(x => x.FundingOfferId)
            .Distinct()
            .ToList();

        if (qaaCandidates.Count == 0)
        {
            return [];
        }

        var fundings = await _context.QaaQualificationFundings
            .Where(qf =>
                sourceQualificationIds.Contains(qf.QaaQualificationId) &&
                fundingOfferIds.Contains(qf.FundingOfferId))
            .ToListAsync(cancellationToken);

        return qaaCandidates
            .Join(
                fundings,
                key => new
                {
                    key.SourceQualificationId,
                    key.FundingOfferId
                },
                funding => new
                {
                    SourceQualificationId = funding.QaaQualificationId,
                    funding.FundingOfferId
                },
                (key, funding) => new RolloverFundingUpdate(
                    RolloverSourceTypes.Qaa,
                    funding.QaaQualificationId,
                    funding.FundingOfferId,
                    key.AcademicYear,
                    funding.EndDate,
                    (endDate, comments, updatedAt) =>
                        funding.Update(funding.StartDate, endDate, funding.FundingStatus, updatedAt, comments)))
            .ToList();
    }
}
