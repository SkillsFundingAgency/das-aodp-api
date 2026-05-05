using MediatR;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Models.Files;
namespace SFA.DAS.AODP.Application.Queries.Files
{

    public class GetFileMetadataQueryHandler : IRequestHandler<GetFileMetadataQuery, BaseMediatrResponse<GetFileMetadataQueryResponse>>
    {
        private readonly IFileRecordRepository _fileRecordRepository;

        public GetFileMetadataQueryHandler(IFileRecordRepository fileRecordRepository)
        {
            _fileRecordRepository = fileRecordRepository;
        }


        public async Task<BaseMediatrResponse<GetFileMetadataQueryResponse>> Handle(
            GetFileMetadataQuery request,
            CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetFileMetadataQueryResponse>
            {
                Success = false
            };

            try
            {
                IEnumerable<FileRecord> records;

                if (request.FileId.HasValue)
                {
                    var record = await _fileRecordRepository.GetByIdAsync(request.FileId.Value);
                    records = record != null
                        ? new[] { record }
                        : Enumerable.Empty<FileRecord>();
                }
                else
                {
                    records = await _fileRecordRepository.GetFilesAsync(
                        request.FileCategory,
                        request.ApplicationId,
                        request.MessageId,
                        request.QuestionId);
                }

                response.Value = new GetFileMetadataQueryResponse
                {
                    Files = records.Select(f => new FileMetadataDto
                    {
                        FileId = f.Id,
                        FileName = f.FileName,
                        ApplicationId = f.ApplicationId,
                        MessageId = f.MessageId,
                        QuestionId = f.QuestionId,
                        BlobContainer = f.BlobContainer,
                        BlobPath = f.BlobPath,
                        ContentType = f.ContentType,
                        IsDownloadable = (f.ScanResult == MalwareScanStatus.Clean)
                    }).ToList()
                };

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.InnerException = ex;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
