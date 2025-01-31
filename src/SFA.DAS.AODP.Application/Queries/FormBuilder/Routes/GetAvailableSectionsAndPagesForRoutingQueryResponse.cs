using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Routes
{
    public class GetAvailableSectionsAndPagesForRoutingQueryResponse : BaseResponse
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
        }


        //TODO: see if can use multiple views or direct entity queries to make mapping simpler
        public static implicit operator GetAvailableSectionsAndPagesForRoutingQueryResponse(List<View_AvailableQuestionsForRouting> entities)
        {
            GetAvailableSectionsAndPagesForRoutingQueryResponse response = new();
            if (entities == null || entities.Count == 0) return response;


            foreach (var sectionId in entities.Select(e => e.SectionId).Distinct())
            {
                var section = entities.First(e => e.SectionId == sectionId);
                var responseSection = new Section { Id = section.SectionId, Order = section.SectionOrder, Title = section.SectionTitle };

                foreach (var pageId in entities.Where(e => e.SectionId == sectionId).Select(p => p.PageId).Distinct())
                {
                    var page = entities.First(e => e.PageId == pageId);
                    responseSection.Pages.Add(new()
                    {
                        Id = page.PageId,
                        Order = page.PageOrder,
                        Title = page.PageTitle,
                    });
                }
                response.Sections.Add(responseSection);
            }

            return response;
        }


    }
}