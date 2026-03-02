namespace FileConverterSuite.Domain.ValueObjects;

public sealed class ConversionFormat
{
    public ConversionFormat(string extension, string displayName)
    {
        Extension = extension.StartsWith(".") ? extension.ToLowerInvariant() : "." + extension.ToLowerInvariant();
        DisplayName = displayName;
    }

    public string Extension { get; }
    public string DisplayName { get; }
}
