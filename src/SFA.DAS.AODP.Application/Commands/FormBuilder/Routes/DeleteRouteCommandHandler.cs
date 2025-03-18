using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Routes
{
    public class DeleteRouteCommandHandler(IRouteRepository _routeRepository, IQuestionRepository _questionRepository) : IRequestHandler<DeleteRouteCommand, BaseMediatrResponse<EmptyResponse>>
    {
        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>()
            {
                Success = false
            };

            try
            {
                if (!await _questionRepository.IsQuestionEditableAsync(request.QuestionId)) throw new RecordLockedException();

                await _routeRepository.DeleteRouteByQuestionIdAsync(request.QuestionId);
                response.Success = true;
            }
            catch (RecordLockedException)
            {
                response.Success = false;
                response.InnerException = new LockedRecordException();
            }
            catch (Exception ex)
            {
                response.InnerException = ex.InnerException;
                response.ErrorMessage = ex.Message;
                response.Success = false;
            }

            return response;
        }
    }
}
