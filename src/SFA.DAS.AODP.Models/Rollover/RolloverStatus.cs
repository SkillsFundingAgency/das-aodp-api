namespace SFA.DAS.AODP.Models.Rollover;

public enum RolloverStatus
{
    None = 0,
    NeedsReview = 1,
    InProgress = 2,
    Extended = 3,
    Excluded = 4,
    Rejected = 5,  
    Ignored = 6,
    Unknown = 99
}