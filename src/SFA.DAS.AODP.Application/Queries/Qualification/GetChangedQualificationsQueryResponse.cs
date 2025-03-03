using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetChangedQualificationsQueryResponse
{
    public List<ChangedQualification> Data { get; set; } = new();

    public class ChangedQualification
    {
        public Guid Id { get; set; }
        public Guid Qan { get; set; }
        public string QualificationName { get; set; } = string.Empty;

        public static implicit operator ChangedQualification(Data.Entities.Qualification.ChangedQualification qualification)
        {
            return (new()
            {
                Id = qualification.Id,
                Qan = qualification.Qan,
                QualificationName = qualification.QualificationName,
            });
        }
    }
}
