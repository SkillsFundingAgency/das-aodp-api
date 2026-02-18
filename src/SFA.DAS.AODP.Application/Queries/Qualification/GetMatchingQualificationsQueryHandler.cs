using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Search;
using SFA.DAS.AODP.Models.Settings;
using System.Text.RegularExpressions;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetMatchingQualificationsQueryHandler : IRequestHandler<GetMatchingQualificationsQuery, BaseMediatrResponse<GetMatchingQualificationsQueryResponse>>
{
    private readonly IQualificationsSearchService _qualificationsSearchService;
    private readonly IQualificationsRepository _qualificationsRepository;
    private readonly ILogger<GetMatchingQualificationsQueryHandler> _logger;
    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(5);
    private readonly bool _fuzzySearchEnabled;

    public GetMatchingQualificationsQueryHandler(
                    IQualificationsSearchService qualificationsSearchService, 
                    IQualificationsRepository qualificationsRepository,
                    IOptions<FuzzySearchSettings> fuzzySearchOptions,
                    ILogger<GetMatchingQualificationsQueryHandler> logger)
    {
        _qualificationsSearchService = qualificationsSearchService;
        _qualificationsRepository = qualificationsRepository;
        _logger = logger;
        _fuzzySearchEnabled = fuzzySearchOptions.Value.Enabled;
    }

    public async Task<BaseMediatrResponse<GetMatchingQualificationsQueryResponse>> Handle(GetMatchingQualificationsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetMatchingQualificationsQueryResponse>();
        try
        {
            IEnumerable<SearchedQualification> qualifications = Enumerable.Empty<SearchedQualification>();

            var trimmed = request.SearchTerm!.Trim();

            var isQualificationReference = Regex.IsMatch(
                trimmed,
                @"^(?=(?:.*\d){5,})(?:[A-Za-z0-9]{8}|\d+\/\d+\/[A-Za-z0-9]+)$",
                RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(5));

            if (isQualificationReference)
            {
                // Normalize: remove any non-alphanumeric characters (removes '/' etc.)
                var normalized = Regex.Replace(trimmed, @"[^A-Za-z0-9]", "", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));

                // Ensure an 8-character alphanumeric string:
                if (normalized.Length < 8)
                {
                    // Pad left with zeros if too short (keeps numeric padding behavior for short refs)
                    normalized = normalized.PadLeft(8, '0');
                }
                else if (normalized.Length > 8)
                {
                    // If longer than 8, take the right-most 8 characters
                    normalized = normalized.Substring(normalized.Length - 8);
                }

                var getByIdTask = _qualificationsRepository.GetByIdAsync(normalized);
                var qualification = await getByIdTask.WaitAsync(OperationTimeout, cancellationToken).ConfigureAwait(false);
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
            else if (!_fuzzySearchEnabled)
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                linkedCts.CancelAfter(OperationTimeout);

                var getByNameTask = _qualificationsRepository.GetSearchedQualificationByNameAsync(trimmed);
                var searched = await getByNameTask.WaitAsync(OperationTimeout, linkedCts.Token).ConfigureAwait(false);
                qualifications = searched ?? Enumerable.Empty<SearchedQualification>();
            }
            else
            {
                // Ensure the search operation is bounded by a timeout: create a linked CTS with OperationTimeout.
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                linkedCts.CancelAfter(OperationTimeout);

                qualifications = _qualificationsSearchService.SearchQualificationsByKeywordAsync(trimmed, linkedCts.Token);
            }

            // Put "No Action Required" items first.
            var noActionGuid = new Guid("00000000-0000-0000-0000-000000000002");

            var orderedQualifications = qualifications
                .OrderBy(q => q.Status != noActionGuid)
                .ThenBy(q => q.QualificationName)
                .ThenBy(q => q.Qan);

            response.Value = new GetMatchingQualificationsQueryResponse
            {
                TotalRecords = qualifications.Count(),
                Skip = request.Skip,
                Take = request.Take,
                Qualifications = orderedQualifications.Select(q => new GetMatchingQualificationsQueryItem
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
        catch (OperationCanceledException)
        {
            // This exception is expected when the operation times out.
            _logger.LogWarning("Search operation timed out after {Timeout} seconds for term: {SearchTerm}", OperationTimeout.TotalSeconds, request.SearchTerm);
            response.Success = false;
            response.ErrorMessage = $"The search operation timed out. Please try again later.";
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