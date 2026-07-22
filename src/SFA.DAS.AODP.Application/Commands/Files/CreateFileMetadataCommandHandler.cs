using MediatR;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Models.Files;

namespace SFA.DAS.AODP.Application.Commands.Files
{
    public class CreateFileMetadataCommandHandler : IRequestHandler<CreateFileMetadataCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IFileRecordRepository _fileRecordRepository;

        public CreateFileMetadataCommandHandler(IFileRecordRepository fileRecordRepository)
        {
            _fileRecordRepository = fileRecordRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(CreateFileMetadataCommand command, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fileMetadata = new FileRecord
                {
                    Id = Guid.NewGuid(),
                    FileCategory = command.FileCategory,
                    ApplicationId = command.ApplicationId,
                    MessageId = command.MessageId,
                    QuestionId = command.QuestionId,
                    FileName = command.FileName,
                    ContentType = command.ContentType,
                    BlobContainer = command.BlobContainer,
                    BlobPath = command.BlobPath,
                    UploadedByDisplayName = command.UploadedBy,
                    UploadedAt = DateTime.UtcNow,
                    ScanResult = MalwareScanStatus.NotScanned,
                };

                await _fileRecordRepository.AddAsync(fileMetadata);

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                response.InnerException = ex;
            }

            return response;
        }
    }
}
                   