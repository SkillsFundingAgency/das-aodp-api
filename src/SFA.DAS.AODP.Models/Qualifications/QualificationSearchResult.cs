﻿using Lucene.Net.Documents;

namespace SFA.DAS.AODP.Models.Qualifications
{
    public class QualificationSearchResult
    {
        public QualificationSearchResult() { }

        public QualificationSearchResult(Document document,
            float score)
        {
            Id = document.GetField(nameof(Id)).GetStringValue();
            QualificationName = document.GetField(nameof(QualificationName)).GetStringValue();
            Qan = document.GetField(nameof(Qan)).GetStringValue();
        }

        public string Id { get; set; }
        public string QualificationName { get; set; }
        public string Qan { get; set; }
    }

}