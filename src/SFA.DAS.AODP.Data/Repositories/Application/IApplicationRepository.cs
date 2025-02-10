







using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationRepository
    {
        Task<Entities.Application.Application> Create(Entities.Application.Application application);
        Task<Entities.Application.Application> GetByIdAsync(Guid applicationId);
        Task<List<Entities.Application.Application>> GetByOrganisationId(Guid organisationId);
        Task<List<View_RemainingPagesBySectionForApplication>> GetRemainingPagesBySectionForApplicationsAsync(Guid applicationId);
    }
}