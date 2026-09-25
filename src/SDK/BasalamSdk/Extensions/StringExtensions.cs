namespace Basalam.SDK.Extensions;

public static class StringExtensions
{
    public static string EnsureTrailingSlash(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.EndsWith("/") ? value : $"{value}/";
    }

    public static string EnsureLeadingSlash(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.StartsWith("/") ? value : $"/{value}";
    }

    public static string RemoveTrailingSlash(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.EndsWith("/") ? value[..^1] : value;
    }
}
