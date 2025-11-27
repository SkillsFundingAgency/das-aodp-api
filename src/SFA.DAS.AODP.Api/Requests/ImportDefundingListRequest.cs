using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.AODP.Api.Requests;

public class ImportDefundingListRequest
{
    [FromForm(Name = "file")]
    public IFormFile? File { get; set; }
}
