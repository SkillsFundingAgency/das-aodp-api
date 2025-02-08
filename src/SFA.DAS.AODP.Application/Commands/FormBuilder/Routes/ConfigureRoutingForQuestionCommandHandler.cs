using MediatR;using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Routes
{
    public class ConfigureRoutingForQuestionCommandHandler(IRouteRepository _routeRepository, IQuestionRepository _questionRepository) : IRequestHandler<ConfigureRoutingForQuestionCommand, BaseMediatrResponse<EmptyResponse>>
    {
        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(ConfigureRoutingForQuestionCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>()
            {
                Success = false
            };

            try
            {
                if (!await _questionRepository.IsQuestionEditable(request.QuestionId)) throw new RecordLockedException();

                List<Route> dbRoutes = await _routeRepository.GetRoutesByQuestionId(request.QuestionId);

                foreach (var requestRotue in request.Routes)
                {
                    var routeEntity = dbRoutes.FirstOrDefault(r => r.SourceOptionId == requestRotue.OptionId);

                    if (routeEntity == null)
                    {
                        routeEntity = new()
                        {
                            SourceOptionId = requestRotue.OptionId,
                            SourceQuestionId = request.QuestionId
                        };
                        dbRoutes.Add(routeEntity);
                    }


                    routeEntity.NextPageId = requestRotue.NextPageId;
                    routeEntity.NextSectionId = requestRotue.NextSectionId;
                    routeEntity.EndSection = requestRotue.EndSection;
                    routeEntity.EndForm = requestRotue.EndForm;
                }
                await _routeRepository.UpsertAsync(dbRoutes);
                response.Success = true;
            }
            catch (RecordLockedException)
            {
                response.Success = false;
                response.InnerException = new LockedRecordException();
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
            }

            return response;
        }
    }
}
