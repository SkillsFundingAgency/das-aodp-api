namespace SFA.DAS.AODP.Models.Application
{
    public static class ReviewerAssignmentRules
    {
        public static bool WouldCauseConflict(string? reviewer1, string? reviewer2)
        {
            if (string.IsNullOrWhiteSpace(reviewer1) || string.IsNullOrWhiteSpace(reviewer2))
                return false;

            return reviewer1.Trim().Equals(reviewer2.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}

