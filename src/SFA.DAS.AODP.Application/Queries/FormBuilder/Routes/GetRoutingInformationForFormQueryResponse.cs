using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{

    public class GetRoutingInformationForFormQueryResponse : BaseResponse
    {
        public List<Section> Sections { get; set; } = new();
        public class Section
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }
            public List<Page> Pages { get; set; } = new();
        }

        public class Page
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }
            public Question Quesiton { get; set; } = new();
        }

        public class Question
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }
            public List<RouteInformation> Routes { get; set; } = new();
        }

        public class RouteInformation
        {
            public Page? NextPage { get; set; }
            public Section? NextSection { get; set; }
            public bool EndForm { get; set; }
            public bool EndSection { get; set; }
            public RadioOptionItem Option { get; set; }
        }

        public class RadioOptionItem
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
            public int Order { get; set; }
        }


        //TODO: see if can use multiple views or direct entity queries to make mapping simpler
        public static GetRoutingInformationForFormQueryResponse Map(List<View_QuestionRoutingDetail> routes)
        {
            GetRoutingInformationForFormQueryResponse response = new();

            // Structure: Sections > Pages > Quesitons > Options > Route
            foreach (var routeSectionId in routes.Select(r => r.SectionId).Distinct())
            {
                var routeSection = routes.Where(r => r.SectionId == routeSectionId).ToList();

                // Build section
                var section = new Section()
                {
                    Id = routeSection[0].SectionId,
                    Title = routeSection[0].SectionTitle,
                    Order = routeSection[0].SectionOrder,
                    Pages = new()
                };
                response.Sections.Add(section);

                foreach (var routePageId in routeSection.Select(r => r.PageId).Distinct())
                {
                    var sectionPage = routeSection.Where(r => r.PageId == routePageId).ToList();

                    // Build page for section with question information (only 1 question per page can have routing)
                    var page = new Page()
                    {
                        Id = sectionPage[0].PageId,
                        Title = sectionPage[0].PageTitle,
                        Order = sectionPage[0].PageOrder,
                        Quesiton = new()
                        {
                            Id = sectionPage[0].QuestionId,
                            Order = sectionPage[0].QuestionOrder,
                            Title = sectionPage[0].QuestionTitle,
                            Routes = new()
                        }
                    };
                    section.Pages.Add(page);

                    // Build routing information per option for question
                    foreach (var option in sectionPage)
                    {
                        page.Quesiton.Routes.Add(new()
                        {
                            EndForm = option.EndForm,
                            EndSection = option.EndSection,
                            NextPage = new()
                            {
                                Id = option.NextPageId ?? default,
                                Order = option.NextPageOrder ?? default,
                                Title = option.NextPageTitle
                            },
                            NextSection = new()
                            {
                                Id = option.NextSectionId ?? default,
                                Order = option.NextSectionOrder ?? default,
                                Title = option.NextSectionTitle
                            },
                            Option = new()
                            {
                                Id = option.OptionId,
                                Order = option.OptionOrder,
                                Value = option.OptionValue
                            }
                        });
                    }

                }
            }

            return response;

        }
    }
}