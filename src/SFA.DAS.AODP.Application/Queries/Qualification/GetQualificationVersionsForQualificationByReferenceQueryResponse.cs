namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetQualificationVersionsForQualificationByReferenceQueryResponse
{
    public Guid Id { get; set; }

    public string QualificationReference { get; set; }

    public string? QualificationName { get; set; }

    public List<QualificationVersions> QualificationVersionsList { get; set; } = new();

    public class QualificationVersions
    {
        public Guid Id { get; set; }

        public Guid QualificationId { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public DateTime UiLastUpdatedDate { get; set; }

        public DateTime InsertedDate { get; set; }

        public int? Version { get; set; }
    }

    public static implicit operator GetQualificationVersionsForQualificationByReferenceQueryResponse(Data.Entities.Qualification.Qualification qualification)
    {
        GetQualificationVersionsForQualificationByReferenceQueryResponse model = new();
        if(qualification != null)
        {
            model.Id = qualification.Id;
            model.QualificationReference = qualification.Qan;
            model.QualificationName = qualification.QualificationName;

            foreach (var item in qualification.QualificationVersions)
            {
                model.QualificationVersionsList.Add(new QualificationVersions
                {
                    Id = item.Id,
                    QualificationId = item.QualificationId,
                    LastUpdatedDate = item.LastUpdatedDate,
                    UiLastUpdatedDate = item.UiLastUpdatedDate,
                    InsertedDate = item.InsertedDate,
                    Version = item.Version
                });
            }
        }

        return model;
    }
}