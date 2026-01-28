using SFA.DAS.AODP.Models.Qualification;

namespace SFA.DAS.AODP.Infrastructure.Services.Interfaces
{
    public interface IQualificationsApi
    {
        Task<QualificationDTO?> GetByQanAsync(
            string qan,
            CancellationToken cancellationToken = default);
    }
}
