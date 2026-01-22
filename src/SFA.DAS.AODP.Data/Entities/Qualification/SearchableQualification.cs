using Lucene.Net.Documents;
using Lucene.Net.Index;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Search
{
    public class SearchableQualification
    {
        public SearchableQualification(QualificationFundingStatus Qualification)
        {
            Id = Qualification.QualificationId;
            Qan = Qualification?.Qan;
            QualificationName = Qualification.QualificationName;
            Status = Qualification.FundedStatus;
        }

        public Guid Id { get; }
        public string Qan { get; }
        public string QualificationName { get; }
        public string Status { get; }


        private const string PhraseSuffix = "Phrase";
        private const string TermSuffix = "Term";
        private const string NGramSuffix = "NGram";

        // phrase
        public static string QualificationNamePhrase => $"{nameof(Qualification.QualificationName)}-{PhraseSuffix}";
        public static string QanPhrase => $"{nameof(Qualification.Qan)}-{PhraseSuffix}";

        // term
        public static string QualificationNameTerm => $"{nameof(Qualification.QualificationName)}-{TermSuffix}";
        public static string QanTerm => $"{nameof(Qualification.Qan)}-{TermSuffix}";

        // n-gram
        public static string QualificationNameNGram => $"{nameof(Qualification.QualificationName)}-{NGramSuffix}";


        public IEnumerable<IIndexableField> GetFields()
        {
            var storedQualificationName = QualificationName ?? string.Empty;
            var storedQan = Qan ?? string.Empty;
            var storedStatus = Status ?? string.Empty;

            return new Field[]
            {
                new StringField(nameof(Id), Id.ToString(), Field.Store.YES),
                new StringField(nameof(QualificationName), storedQualificationName, Field.Store.YES),
                new StringField(nameof(Qan), storedQan, Field.Store.YES),
                new StringField(nameof(Status), storedStatus, Field.Store.YES),
                // phrase
                new TextField(QualificationNamePhrase, QualificationName ?? "", Field.Store.NO) {Boost = 1000f},
                // term
                new TextField(QualificationNameTerm, QualificationName ?? "", Field.Store.NO) {Boost = 40f},
                // ngram
                new TextField(QualificationNameNGram, QualificationName ?? "", Field.Store.NO) {Boost = 10f}
            };
            //return new Field[]
            //{
            //    new StringField(nameof(Id), Id.ToString(), Field.Store.YES),
            //    new StringField(nameof(QualificationName), QualificationName, Field.Store.YES),
            //    new StringField(nameof(Qan), Qan, Field.Store.YES),
            //    new StringField(nameof(Status), Status, Field.Store.YES),
            //    // phrase
            //    new TextField(QualificationNamePhrase, QualificationName ?? "", Field.Store.NO) {Boost = 1000f},
            //    // term
            //    new TextField(QualificationNameTerm, QualificationName ?? "", Field.Store.NO) {Boost = 40f},
            //    // ngram
            //    new TextField(QualificationNameNGram, QualificationName ?? "", Field.Store.NO) {Boost = 10f}
            //};
        }
    }
}