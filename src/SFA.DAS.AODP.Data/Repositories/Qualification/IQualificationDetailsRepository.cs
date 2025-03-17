using SFA.DAS.AODP.Data.Entities.Qualification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public interface IQualificationDetailsRepository
{
    Task<QualificationVersions> GetQualificationDetailsByIdAsync(string qualificationReference);
}
