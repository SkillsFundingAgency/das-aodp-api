namespace SFA.DAS.AODP.Application.Exceptions;

public class NotFoundException(Guid id) : ApplicationExceptionBase
{
    public Guid Id { get; set; } = id;
}

public class NotFoundWithNameException(string name) : ApplicationExceptionBase
{
    public string Name { get; set; } = name;
}
