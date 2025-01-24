namespace SFA.DAS.AODP.Data.Exceptions;

/// <summary>
/// Thrown when a DB record is created referencing a foreign key that doesn't exist. 
/// </summary>
public class NoForeignKeyException(Guid foreignKey) : RepositoryExceptionBase
{
    public Guid ForeignKey { get; set; } = foreignKey;
}
