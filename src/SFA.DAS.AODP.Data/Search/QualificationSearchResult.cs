using Lucene.Net.Documents;

namespace SFA.DAS.AODP.Data.Search
{
    public class QualificationSearchResult
    {
        public QualificationSearchResult() { }

        public QualificationSearchResult(Document document,
            float score)
        {
            QualificationUId = document.GetField(nameof(QualificationUId)).GetStringValue();
            Score = score;
        }

        public string QualificationUId { get; set; }
        public float Score { get; set; }
    }

}