using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

using SFA.DAS.AODP.Models.Forms.FormBuilder;

public class GetAllSectionsQueryResponse : BaseResponse
{
    public List<Section> Data { get; set; }
}
