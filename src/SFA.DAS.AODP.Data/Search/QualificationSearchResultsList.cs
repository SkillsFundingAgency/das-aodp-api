namespace SFA.DAS.AODP.Data.Search
{
    public class QualificationSearchResultsList
    {
        public int TotalCount { get; set; }
        public IEnumerable<QualificationSearchResult> Qualifications { get; set; } = new List<QualificationSearchResult>();
    }
}
