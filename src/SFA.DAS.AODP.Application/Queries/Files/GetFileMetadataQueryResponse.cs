using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Queries.Files
{
    [ExcludeFromCodeCoverage]
    public class GetFileMetadataQueryResponse
    {
        public List<FileMetadataDto> Files { get; init; } = new();
    }

}
