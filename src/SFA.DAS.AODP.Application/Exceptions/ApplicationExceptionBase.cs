namespace SFA.DAS.AODP.Application.Exceptions;

public class ApplicationExceptionBase : Exception
{
    public ApplicationExceptionBase() : base() { }
    public ApplicationExceptionBase(string message) : base(message) { }
    public ApplicationExceptionBase(string message, Exception inner) : base(message, inner) { }
}
