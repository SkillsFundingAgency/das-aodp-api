using Azure.Core;
using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetChangedQualificationsQueryHandler(IChangedQualificationsRepository qualificationsRepository) : IRequestHandler<GetChangedQualificationsQuery, BaseMediatrResponse<GetChangedQualificationsQueryResponse>>
{
    private readonly IChangedQualificationsRepository _changedQualificationsRepository = qualificationsRepository;

    public async Task<BaseMediatrResponse<GetChangedQualificationsQueryResponse>> Handle(GetChangedQualificationsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetChangedQualificationsQueryResponse>();
        response.Success = false;

        try
        {
            var result = await _changedQualificationsRepository.GetAllChangedQualificationsAsync(
                request.Skip,
                request.Take,
                new QualificationsFilter()
                {
                    Name = request.Name,
                    Organisation = request.Organisation,
                    QAN = request.QAN
                });
            if (result != null)
            {
                response.Value = new GetChangedQualificationsQueryResponse()
                {
                    Data = result.Data,
                    Skip = result.Skip,
                    Take = result.Take,
                    TotalRecords = result.TotalRecords
                };
            response.Success = true;
        }
            else
            {
                response.Success = false;
                response.ErrorMessage = "No new qualifications found.";
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }

        return response;
    }

}
