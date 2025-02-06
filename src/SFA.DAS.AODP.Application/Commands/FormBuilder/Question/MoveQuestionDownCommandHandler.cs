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

public class MoveQuestionDownCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<MoveQuestionDownCommand, MoveQuestionDownCommandResponse>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<MoveQuestionDownCommandResponse> Handle(MoveQuestionDownCommand request, CancellationToken cancellationToken)
    {
        var response = new MoveQuestionDownCommandResponse()
        {
        };

        try
        {
            var res = await _questionRepository.MoveQuestionOrderDown(request.QuestionId);
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
