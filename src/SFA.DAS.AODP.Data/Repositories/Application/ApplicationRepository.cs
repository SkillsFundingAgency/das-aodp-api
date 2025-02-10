using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Exceptions;
using System;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Data.Entities.Application.Application>> GetByOrganisationId(Guid organisationId)
        {
            return await _context.Applications
                //.Where(v => v.OrganisationId == organisationId) //TODO: add filter back when org code setup
                .ToListAsync();

        }

        public async Task<Data.Entities.Application.Application> Create(Data.Entities.Application.Application application)
        {
            application.Id = Guid.NewGuid();
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<Entities.Application.Application> GetByIdAsync(Guid applicationId)
        {
            var res = await _context.Applications.FirstOrDefaultAsync(v => v.Id == applicationId);
            return res is null ? throw new RecordNotFoundException(applicationId) : res;
        }

        public async Task<List<View_RemainingPagesBySectionForApplication>> GetRemainingPagesBySectionForApplicationsAsync(Guid applicationId)
        {
            return await _context.View_RemainingPagesBySectionForApplication.Where(a => a.ApplicationId == applicationId).ToListAsync();
        }
    }
}
