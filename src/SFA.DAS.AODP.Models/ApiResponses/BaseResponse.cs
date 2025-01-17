namespace SFA.DAS.AODP.Models.ApiResponses;

public abstract class BaseResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}