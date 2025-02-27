using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application;

public class ApplicationMessagesRepository : IApplicationMessagesRepository
{
    private readonly IApplicationDbContext _context;

    public ApplicationMessagesRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Message>> GetMessagesByApplicationIdAsync(Guid applicationId)
    {
        return await _context.ApplicationMessages
                .Where(m => m.ApplicationId == applicationId)
                .ToListAsync();
    }
}