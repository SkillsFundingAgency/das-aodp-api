using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Models.Qualifications
{
    public class QualificationSearchResultsList
    {
        public int TotalCount { get; set; }
        public IEnumerable<QualificationSearchResult> Qualifications { get; set; } = new List<QualificationSearchResult>();
    }
}