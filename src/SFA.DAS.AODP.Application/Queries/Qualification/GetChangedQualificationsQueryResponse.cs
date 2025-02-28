using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetChangedQualificationsQueryResponse
{
    public List<Qualification> Data { get; set; } = new();

    public class Qualification
    {
        public Guid Id { get; set; }
        public Guid Qan { get; set; }
        public string QualificationName { get; set; } = string.Empty;

        public static implicit operator Qualification(Data.Entities.Qualification.ChangedQualification qualification)
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
