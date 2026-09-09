using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;

namespace SFA.DAS.AODP.Application.Commands.Application.Application
{
    public class DeleteApplicationCommandHandler : IRequestHandler<DeleteApplicationCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IFileRecordRepository _fileRecordRepository;
        private readonly IBlobStorageService _blobStorageService;

        public DeleteApplicationCommandHandler(IApplicationRepository applicationRepository, IFileRecordRepository fileRecordRepository, IBlobStorageService blobStorageService)
        {
            _applicationRepository = applicationRepository;
            _fileRecordRepository = fileRecordRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
                if (application.Submitted == true) throw new InvalidOperationException("The application has been submitted");


                var files = await _fileRecordRepository.GetByApplicationIdAsync(request.ApplicationId);

                foreach (var file in files)
                {
                    await _blobStorageService.DeleteAsync(
                        file.BlobContainer,
                        file.BlobPath);

                    await _fileRecordRepository.DeleteAsync(file.Id);
                }

                await _applicationRepository.DeleteAsync(application);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.InnerException = ex;
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
