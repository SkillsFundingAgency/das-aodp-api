using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Application;

public class EditApplicationCommandHandler : IRequestHandler<EditApplicationCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IApplicationRepository _applicationRepository;

    public EditApplicationCommandHandler(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(EditApplicationCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (application.Submitted == true) throw new RecordLockedException();

            application.Name = request.Title;
            application.QualificationNumber = request.QualificationNumber;
            application.Owner = request.Owner;

            await _applicationRepository.UpdateAsync(application);
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