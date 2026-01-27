using Lucene.Net.Store;

namespace SFA.DAS.AODP.Data.Search
{
    public interface IDirectoryFactory
    {
        BaseDirectory GetDirectory();
    }
}