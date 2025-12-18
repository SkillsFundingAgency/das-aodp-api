namespace SFA.DAS.AODP.Application.Queries.Qualification
{

    public class GetMatchingQualificationsQueryResponse
    {
        public List<GetMatchingQualificationsQueryItem> Qualifications { get; set; } = new();
    }

    public class GetMatchingQualificationsQueryItem
    {
        public Guid Id { get; set; }
        public string Qan { get; set; } = null!;
        public string? QualificationName { get; set; }
    }

}