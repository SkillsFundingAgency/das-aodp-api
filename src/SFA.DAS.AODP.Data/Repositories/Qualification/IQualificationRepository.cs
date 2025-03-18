
namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IQualificationRepository
    {
        Task<Entities.Qualification.Qualification> GetByIdAsync(string qualificationReference);
    }
}