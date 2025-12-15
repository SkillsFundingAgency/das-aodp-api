namespace SFA.DAS.AODP.Data.Search
{
    public interface ISearchManager
    {
        QualificationSearchResultsList Query(string searchTerm);
    }
}

