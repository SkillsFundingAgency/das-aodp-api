using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Files
{
    [ExcludeFromCodeCoverage]
    public class DeleteFileCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
    {
        public Guid FileId { get; set; }
    }
}
