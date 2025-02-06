using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;

public class GetQuestionByIdQueryHandler(IQuestionRepository _QuestionRepository) : IRequestHandler<GetQuestionByIdQuery, GetQuestionByIdQueryResponse>
{
    public async Task<GetQuestionByIdQueryResponse> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetQuestionByIdQueryResponse();
        response.Success = false;
        try
        {
            var question = await _QuestionRepository.GetQuestionByIdAsync(request.QuestionId);

            response = question;
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
