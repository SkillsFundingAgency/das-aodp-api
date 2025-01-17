using MediatR;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQuery : IRequest<GetAllSectionsQueryResponse>
{
    public Guid FormId { get; set; }
}
