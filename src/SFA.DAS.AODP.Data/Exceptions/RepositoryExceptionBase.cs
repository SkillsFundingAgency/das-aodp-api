namespace SFA.DAS.AODP.Data.Exceptions;

/// <summary>
/// Base exception for all repositories. 
/// </summary>
public class RepositoryExceptionBase : Exception
{
    public RepositoryExceptionBase() : base() { }
    public RepositoryExceptionBase(string message) : base(message) { }
    public RepositoryExceptionBase(string message, Exception inner) : base(message, inner) { }
}
