using RestEase;
using SFA.DAS.AODP.Models.Qualification;

namespace SFA.DAS.AODP.Infrastructure.Clients.Ofqual
{
    public interface IOfqualRegisterApi
    {
        [Header("Ocp-Apim-Subscription-Key")]
        string SubscriptionKey { get; set; }

        [Get("gov/Qualifications/{qan}")]
        Task<QualificationDTO?> GetQualificationByQanAsync([Path] string qan);
    }
}
