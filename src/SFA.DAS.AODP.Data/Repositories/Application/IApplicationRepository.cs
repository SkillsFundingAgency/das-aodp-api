





namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationRepository
    {
        Task<Entities.Application.Application> Create(Entities.Application.Application application);
    }
}