using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.Exceptions;

public class RepositoryExceptionBase : Exception
{
    public RepositoryExceptionBase() : base() { }
    public RepositoryExceptionBase(string message) : base(message) { }
    public RepositoryExceptionBase(string message, Exception inner) : base(message, inner) { }
}
