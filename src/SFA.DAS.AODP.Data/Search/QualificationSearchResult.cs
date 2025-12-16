using Lucene.Net.Documents;

namespace SFA.DAS.AODP.Data.Search
{
    public class QualificationSearchResult
    {
        public QualificationSearchResult() { }

        public QualificationSearchResult(Document document,
            float score)
        {
            Id = document.GetField(nameof(Id)).GetStringValue();
            Score = score;
        }

        public string Id { get; set; }
        public float Score { get; set; }
    }

}