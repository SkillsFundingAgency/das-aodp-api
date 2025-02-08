using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;

public class GetQuestionByIdQueryHandler(IQuestionRepository _QuestionRepository, IFormVersionRepository _formVersionRepository)
    : IRequestHandler<GetQuestionByIdQuery, BaseMediatrResponse<GetQuestionByIdQueryResponse>>
{
    public async Task<BaseMediatrResponse<GetQuestionByIdQueryResponse>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQuestionByIdQueryResponse>();
        response.Success = false;
        try
        {
            var question = await _QuestionRepository.GetQuestionByIdAsync(request.QuestionId);

            response.Value = question;
            response.Value.Editable = await _formVersionRepository.IsFormVersionEditable(request.FormVersionId);
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
