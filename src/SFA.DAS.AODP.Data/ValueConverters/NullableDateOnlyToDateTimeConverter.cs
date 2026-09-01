using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SFA.DAS.AODP.Data.ValueConverters;

[ExcludeFromCodeCoverage]
public class NullableDateOnlyToDateTimeConverter() : ValueConverter<DateOnly?, DateTime?>(
    dateOnly => dateOnly.HasValue ? dateOnly.Value.ToDateTime(TimeOnly.MinValue) : null,
    dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null);