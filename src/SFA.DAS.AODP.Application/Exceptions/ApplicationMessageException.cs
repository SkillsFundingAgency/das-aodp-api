namespace SFA.DAS.AODP.Application.Exceptions;

public class ApplicationMessageException : ApplicationExceptionBase
{
    public ApplicationMessageException(string message, Exception? inner = null)
        : base(message, inner) { }
}

