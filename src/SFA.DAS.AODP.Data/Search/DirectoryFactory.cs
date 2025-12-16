using Lucene.Net.Store;

namespace SFA.DAS.AODP.Data.Search
{
    public class DirectoryFactory : IDirectoryFactory
    {
        private BaseDirectory _baseDirectory;

        public BaseDirectory GetDirectory()
        {
            return _baseDirectory ?? (_baseDirectory = new RAMDirectory());
        }
    }
}
