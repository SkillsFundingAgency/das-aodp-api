﻿using System.Text.Json.Serialization;

namespace SFA.DAS.AODP.Application;

public class BaseMediatrResponse<T> where T : class, new()
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public T Value { get; set; } = new();
    [JsonIgnore]
    public Exception? InnerException { get; set; }
}
