namespace SFA.DAS.AODP.Application.Exceptions;

public class NotFoundException(Guid id) : ApplicationExceptionBase
{
    public Guid Id { get; set; } = id;
}
