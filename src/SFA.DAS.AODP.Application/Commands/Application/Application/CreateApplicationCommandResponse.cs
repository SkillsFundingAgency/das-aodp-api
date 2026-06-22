using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Application;

[ExcludeFromCodeCoverage]
public class CreateApplicationCommandResponse
{
    public Guid Id { get; set; }
    public bool? IsQanValid { get; set; } 
    public string? QanValidationMessage { get; set; }
}