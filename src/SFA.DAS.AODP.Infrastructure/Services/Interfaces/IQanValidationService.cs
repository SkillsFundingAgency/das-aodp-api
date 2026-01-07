using SFA.DAS.AODP.Models.Validation;

namespace SFA.DAS.AODP.Infrastructure.Services.Interfaces
{
    public interface IQanValidationService
    {
        Task<QanValidationResult> ValidateAsync(
            string qan,
            string qualificationTitle,
            string awardingOrganisation,
            CancellationToken cancellationToken = default);
    }
}
