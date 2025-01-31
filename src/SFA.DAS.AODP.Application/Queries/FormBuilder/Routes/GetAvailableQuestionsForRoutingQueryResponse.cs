using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetAvailableQuestionsForRoutingQueryResponse : BaseResponse
    {
        public List<Question> Questions { get; set; } = new();

        public class Question
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }

        }

        public static implicit operator GetAvailableQuestionsForRoutingQueryResponse(List<View_AvailableQuestionsForRouting> items)
        {
            GetAvailableQuestionsForRoutingQueryResponse response = new();
            foreach (var item in items)
            {
                response.Questions.Add(new Question
                {
                    Id = item.QuestionId,
                    Order = item.QuestionOrder,
                    Title = item.QuestionTitle
                });
            }

            return response;
        }
    }
}

