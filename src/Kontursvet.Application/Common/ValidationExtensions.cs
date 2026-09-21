namespace Kontursvet.Application.Common;

internal static class ValidationExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
}