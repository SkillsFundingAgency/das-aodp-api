namespace SFA.DAS.AODP.Models.Rollover;

public enum RolloverStatus
{
    None = 0,
    NeedsReview = 1,
    InProgress = 2,
    Extended = 3,
    Rejected = 4,  
    Ignored = 5,
    Unknown = 99
}