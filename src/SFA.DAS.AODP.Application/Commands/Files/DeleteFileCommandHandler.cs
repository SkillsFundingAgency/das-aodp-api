using MediatR;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;

namespace SFA.DAS.AODP.Application.Commands.Files
{
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IFileRecordRepository _fileRecordRepository;
        private readonly IBlobStorageService _blobStorageService;

        public DeleteFileCommandHandler(IFileRecordRepository fileRecordRepository, IBlobStorageService blobStorageService)
        {
            _fileRecordRepository = fileRecordRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var fileMetadata = await _fileRecordRepository.GetByIdAsync(request.FileId);


                if (fileMetadata == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "File not found";
                    return response;
                }

                await _blobStorageService.DeleteAsync(
                            fileMetadata.BlobContainer,
                            fileMetadata.BlobPath);

                await _fileRecordRepository.DeleteAsync(request.FileId);

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
