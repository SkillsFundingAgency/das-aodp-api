using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Application.Exceptions;

public class DependantNotFoundException(Guid dependantId) : ApplicationExceptionBase
{
    public Guid DependantId { get; set; } = dependantId;
}
