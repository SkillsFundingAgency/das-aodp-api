using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Search;
using System.Runtime.CompilerServices;
using data = SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetMatchingQualificationsQueryHandler : IRequestHandler<GetMatchingQualificationsQuery, BaseMediatrResponse<GetMatchingQualificationsQueryResponse>>
{
    private readonly IQualificationsSearchService _qualificationsSearchService;
    private readonly IQualificationsRepository _qualificationsRepository;
    private readonly ILogger<GetMatchingQualificationsQueryHandler> _logger;

    public GetMatchingQualificationsQueryHandler(IQualificationsSearchService qualificationsSearchService, IQualificationsRepository qualificationsRepository, ILogger<GetMatchingQualificationsQueryHandler> logger)
    {
        _qualificationsSearchService = qualificationsSearchService;
        _qualificationsRepository = qualificationsRepository;
        _logger = logger;
    }
    public async Task<BaseMediatrResponse<GetMatchingQualificationsQueryResponse>> Handle(GetMatchingQualificationsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetMatchingQualificationsQueryResponse>();
        try
        {
            IEnumerable<SearchedQualification> qualifications = Enumerable.Empty<SearchedQualification>();

            var trimmed = request.SearchTerm!.Trim();

            // Detect qualification reference: either exactly 8 digits or digits/digits/digits
            var isQualificationReference = System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^(?:\d{8}|\d+\/\d+\/\d+)$");

            if (isQualificationReference)
            {
                // Normalize: remove non-digit characters (removes '/' etc.)
                var normalizedDigits = System.Text.RegularExpressions.Regex.Replace(trimmed, @"\D", "");

                // Ensure an 8-digit numeric string:
                if (normalizedDigits.Length < 8)
                {
                    // Pad left with zeros if too short
                    normalizedDigits = normalizedDigits.PadLeft(8, '0');
                }
                else if (normalizedDigits.Length > 8)
                {
                    // If longer than 8, take the right-most 8 digits
                    normalizedDigits = normalizedDigits.Substring(normalizedDigits.Length - 8);
                }

                var qualification = await _qualificationsRepository.GetByIdAsync(normalizedDigits);
                if (qualification != null)
                {
                    qualifications = new List<SearchedQualification> { new SearchedQualification {
                        Id = qualification.Id,
                        Qan = qualification.Qan,
                        QualificationName = qualification.QualificationName,
                        Status = qualification.QualificationVersions.OrderByDescending(qv => qv.Version).FirstOrDefault()?.ProcessStatusId ?? Guid.Empty
                    } };
                }
            }
            else
            {
                // Use Take from request for pagination; Search service supports 'take' and cancellation token.
                qualifications = _qualificationsSearchService.SearchQualificationsByKeywordAsync(trimmed, cancellationToken);
            }

            response.Value = new GetMatchingQualificationsQueryResponse
            {
                TotalRecords = qualifications.Count(),
                Skip = request.Skip,
                Take = request.Take,
                Qualifications = qualifications.Select(q => new GetMatchingQualificationsQueryItem
                {
                    Id = q.Id,
                    Qan = q.Qan,
                    QualificationName = q.QualificationName,
                    Status = q.Status
                })
                .Skip(request.Skip ?? 0)
                .Take(request.Take ?? 25)
                .ToList()
            };
            response.Success = true;
        }
        catch (RecordWithNameNotFoundException ex)
        {
            _logger.LogError(ex, "Error occurred while searching for qualifications with term: {SearchTerm}", request.SearchTerm);
            response.Success = false;
            response.ErrorMessage = $"No qualifications were found matching search term: {request.SearchTerm}";
            response.ErrorCode = "NO_MATCHES";
            response.Value = new GetMatchingQualificationsQueryResponse
            {
                TotalRecords = 0,
                Skip = request.Skip,
                Take = request.Take,
                Qualifications = new List<GetMatchingQualificationsQueryItem>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for qualifications with term: {SearchTerm}", request.SearchTerm);
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
            response.Value = new GetMatchingQualificationsQueryResponse
            {
                TotalRecords = 0,
                Skip = request.Skip,
                Take = request.Take,
                Qualifications = new List<GetMatchingQualificationsQueryItem>()
            };
        }

        return response;
    }
}