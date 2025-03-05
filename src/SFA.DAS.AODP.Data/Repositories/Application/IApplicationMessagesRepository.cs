using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application;

public interface IApplicationMessagesRepository
{
    Task<List<Message>> GetMessagesByApplicationIdAsync(Guid applicationId);
    Task<Guid> CreateAsync(Message message);
}