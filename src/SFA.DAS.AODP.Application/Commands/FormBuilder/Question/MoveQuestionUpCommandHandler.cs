using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder;

public class MoveQuestionUpCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<MoveQuestionUpCommand, MoveQuestionUpCommandResponse>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<MoveQuestionUpCommandResponse> Handle(MoveQuestionUpCommand request, CancellationToken cancellationToken)
    {
        var response = new MoveQuestionUpCommandResponse()
        {
        };

        try
        {
            var res = await _questionRepository.MoveQuestionOrderUp(request.QuestionId);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
