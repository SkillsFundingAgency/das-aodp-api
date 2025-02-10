using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetAllFormVersionsQuery : IRequest<BaseMediatrResponse<GetAllFormVersionsQueryResponse>> { }