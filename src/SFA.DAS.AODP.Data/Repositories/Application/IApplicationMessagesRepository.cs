using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application;

public interface IApplicationMessagesRepository
{
    Task<List<Message>> GetMessagesByApplicationIdAndUserTypeAsync(Guid applicationId, UserType userType);
    Task<Guid> CreateAsync(Message message);
    Task<Message> GetByIdAsync(Guid messageId);
}