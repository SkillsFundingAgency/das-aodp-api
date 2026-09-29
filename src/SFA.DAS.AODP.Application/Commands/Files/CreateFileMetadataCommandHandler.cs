using MediatR;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Models.Files;

namespace SFA.DAS.AODP.Application.Commands.Files
{
    public class CreateFileMetadataCommandHandler : IRequestHandler<CreateFileMetadataCommand, BaseMediatrResponse<EmptyResponse>>
    {
        // Categories representing a single, ongoing import file (as opposed to per-application
        // uploads such as question uploads or message attachments, where multiple distinct files
        // legitimately coexist). Re-uploading one of these categories replaces the existing record
        // in place, rather than accumulating a new row per upload.
        private static readonly HashSet<FileCategory> SingleRecordCategories =
        [
            FileCategory.Pldns,
            FileCategory.DefundingList
        ];

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
                FileRecord? existing = null;

                if (SingleRecordCategories.Contains(command.FileCategory))
                {
                    existing = await _fileRecordRepository.GetByCategoryAsync(command.FileCategory);
                }

                if (existing != null)
                {
                    existing.FileName = command.FileName;
                    existing.ContentType = command.ContentType;
                    existing.BlobContainer = command.BlobContainer;
                    existing.BlobPath = command.BlobPath;
                    existing.UploadedByDisplayName = command.UploadedBy;
                    existing.UploadedAt = DateTime.UtcNow;
                    existing.ScanResult = MalwareScanStatus.NotScanned;
                    existing.LastScanAt = null;

                    await _fileRecordRepository.UpdateAsync(existing);
                }
                else
                {
                    var fileMetadata = new FileRecord
                    {
                        Id = command.Id ?? Guid.NewGuid(),
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
                }

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
                   