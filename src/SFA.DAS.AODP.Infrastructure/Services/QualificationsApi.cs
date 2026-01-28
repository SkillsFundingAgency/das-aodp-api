using SFA.DAS.AODP.Infrastructure.Clients.Ofqual;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Qualification;

namespace SFA.DAS.AODP.Infrastructure.Services
{
    public class QualificationsApi : IQualificationsApi
    {
        private readonly IOfqualRegisterApi _ofqualRegisterApi;

        public QualificationsApi(IOfqualRegisterApi ofqualRegisterApi)
        {
            _ofqualRegisterApi = ofqualRegisterApi;
        }

        public Task<QualificationDTO?> GetByQanAsync(string qan, CancellationToken cancellationToken = default)
        {
            return _ofqualRegisterApi.GetQualificationByQanAsync(qan);
        }
    }
}
