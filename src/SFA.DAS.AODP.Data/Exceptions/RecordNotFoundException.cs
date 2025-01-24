namespace SFA.DAS.AODP.Data.Exceptions;

/// <summary>
/// Thrown when a database record cannot be found for a given Id. 
/// </summary>
/// <param name="id"></param>
public class RecordNotFoundException(Guid id) : Exception
{
    public Guid Id { get; set; } = id;
}
