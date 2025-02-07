using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetRoutingInformationForQuestionQueryResponse : BaseResponse
    {
        public Guid QuestionId { get; set; }

        public string QuestionTitle { get; set; }
        public string SectionTitle { get; set; }
        public string PageTitle { get; set; }

        public List<RouteInformation> Routes { get; set; } = new();
        public List<RadioOptionItem> RadioOptions { get; set; } = new();
        public List<Page> NextPages { get; set; } = new();
        public List<Section> NextSections { get; set; } = new();
        public bool Editable { get; set; }

        public class RadioOptionItem
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
            public int Order { get; set; }
        }

        public class Section
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }

        }

        public class Page
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }
        }

        public class RouteInformation
        {
            public Guid Id { get; set; }
            public Guid SourceQuestionId { get; set; }
            public Guid? NextPageId { get; set; }
            public Guid? NextSectionId { get; set; }
            public Guid OptionId { get; set; }
            public bool EndSection { get; set; }
            public bool EndForm { get; set; }

        }

        public static GetRoutingInformationForQuestionQueryResponse Map(Question question, List<Data.Entities.FormBuilder.Section> sections, List<Data.Entities.FormBuilder.Page> pages, bool editable)
        {
            GetRoutingInformationForQuestionQueryResponse response = new()
            {
                QuestionId = question.Id,
                PageTitle = question.Page.Title,
                QuestionTitle = question.Title,
                SectionTitle = question.Page.Section.Title,
                Editable = editable
            };

            foreach (Data.Entities.FormBuilder.Section section in sections ?? [])
            {
                response.NextSections.Add(new()
                {
                    Id = section.Id,
                    Title = section.Title,
                    Order = section.Order,
                });
            }


            foreach (Data.Entities.FormBuilder.Page page in pages ?? [])
            {
                response.NextPages.Add(new()
                {
                    Id = page.Id,
                    Title = page.Title,
                    Order = page.Order,
                });
            }



            foreach (QuestionOption option in question.QuestionOptions ?? [])
            {
                response.RadioOptions.Add(new()
                {
                    Id = option.Id,
                    Value = option.Value,
                    Order = option.Order,
                });
            }

            foreach (Data.Entities.FormBuilder.Route route in question.Routes ?? [])
            {
                response.Routes.Add(new()
                {
                    Id = route.Id,
                    EndForm = route.EndForm,
                    EndSection = route.EndSection,
                    NextPageId = route.NextPageId,
                    NextSectionId = route.NextSectionId,
                    OptionId = route.SourceOptionId,
                    SourceQuestionId = route.SourceQuestionId,
                });
            }


            return response;

        }
    }
}