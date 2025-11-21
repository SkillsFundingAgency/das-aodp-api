using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application;

public class ApplicationMessagesRepository : IApplicationMessagesRepository
{
    private readonly IApplicationDbContext _context;

    public ApplicationMessagesRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Message>> GetMessagesByApplicationIdAndUserTypeAsync(Guid applicationId, UserType userType)
    {
        IQueryable<Message> query = _context.Messages;

        switch (userType)
        {
            case UserType.Qfau:
                query = query.Where(m => m.ApplicationId == applicationId && m.SharedWithDfe == true);
                break;
            case UserType.Ofqual:
                query = query.Where(m => m.ApplicationId == applicationId && m.SharedWithOfqual == true);
                break;
            case UserType.SkillsEngland:
                query = query.Where(m => m.ApplicationId == applicationId && m.SharedWithSkillsEngland == true);
                break;
            case UserType.AwardingOrganisation:
                query = query.Where(m => m.ApplicationId == applicationId && m.SharedWithAwardingOrganisation == true);
                break;
        }

        return await query.ToListAsync();
    }

    public async Task<Message> GetByIdAsync(Guid messageId)
    {
        var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
        if (message == null) throw new RecordNotFoundException(messageId);
        return message;
    }

    public async Task<Guid> CreateAsync(Message message)
    {
        message.Id = Guid.NewGuid();
        message.SentAt = DateTime.UtcNow;
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        return message.Id;
    }

    public async Task<Message?> GetLatestByTypeAsync(Guid applicationId, MessageType type)
    {
        return await _context.Messages
            .Where(m => m.ApplicationId == applicationId && m.Type == type)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync();
    }
}