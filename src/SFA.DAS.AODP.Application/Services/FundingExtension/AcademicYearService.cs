using SFA.DAS.AODP.Infrastructure.Services.Interfaces;

namespace SFA.DAS.AODP.Application.Services.FundingExtension
{
    public interface IAcademicYearService
    {
        string GetCurrentAcademicYear();
    }

    public class AcademicYearService : IAcademicYearService
    {
        private readonly ISystemClockService _clock;

        public AcademicYearService(ISystemClockService clock)
        {
            _clock = clock;
        }

        /// <summary>
        /// Calculates the current academic year based on today's date.
        /// Academic year runs from 1 August to 31 July.
        /// Example:
        /// - If today is Jan 2026 → "2025/26"
        /// - If today is Sep 2026 → "2026/27"
        /// </summary>
        public string GetCurrentAcademicYear()
        {
            var today = _clock.UtcNow;

            // Academic year starts on 1 August
            var startYear = today.Month >= 8 ? today.Year : today.Year - 1;
            var endYearShort = (startYear + 1) % 100;

            return $"{startYear}/{endYearShort:D2}";
        }
    }
}

