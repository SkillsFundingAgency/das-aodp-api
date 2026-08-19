using MediatR;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Application.Commands.Application.Application
{
    public class DeleteApplicationCommandHandler : IRequestHandler<DeleteApplicationCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly IApplicationRepository _applicationRepository;

        public DeleteApplicationCommandHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();

            try
            {
                var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);

                var isQfauUser = Enum.TryParse(request.UserType, true, out UserType userType) && userType == UserType.Qfau;

                if (application.Submitted == true && !isQfauUser) throw new InvalidOperationException("The application has been submitted");

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
