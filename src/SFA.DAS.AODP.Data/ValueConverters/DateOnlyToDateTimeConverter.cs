using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SFA.DAS.AODP.Data.ValueConverters;

[ExcludeFromCodeCoverage]
public class DateOnlyToDateTimeConverter() : ValueConverter<DateOnly, DateTime>(
    dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
    dateTime => DateOnly.FromDateTime(dateTime));