using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Settings;

[ExcludeFromCodeCoverage]
public class BlobStorageSettings
{
	public string ConnectionString { get; set; } = string.Empty;
	public string ContainerName { get; set; } = string.Empty;
}