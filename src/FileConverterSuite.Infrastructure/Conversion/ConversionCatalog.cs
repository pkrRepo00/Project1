using FileConverterSuite.Domain.Enums;

namespace FileConverterSuite.Infrastructure.Conversion;

public static class ConversionCatalog
{
    public static IReadOnlyCollection<PluginDefinition> BuildDefinitions(string toolsRoot)
    {
        var definitions = new List<PluginDefinition>();

        definitions.Add(new PluginDefinition("document.office", "Office/PDF Converter", ConversionCategory.Document,
            Path.Combine(toolsRoot, "LibreOffice", "program", "soffice.exe"),
            BuildPairs(new[] { ".pdf", ".docx", ".xlsx", ".pptx", ".odt", ".ods", ".odp", ".rtf", ".txt", ".csv" })));

        definitions.Add(new PluginDefinition("image.magick", "Image Converter", ConversionCategory.Image,
            Path.Combine(toolsRoot, "ImageMagick", "magick.exe"),
            BuildPairs(new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif", ".tiff", ".heic", ".avif", ".ico" })));

        definitions.Add(new PluginDefinition("archive.7zip", "Archive Utility", ConversionCategory.Archive,
            Path.Combine(toolsRoot, "7zip", "7z.exe"),
            BuildPairs(new[] { ".zip", ".7z", ".rar", ".tar", ".gz", ".bz2" })));

        definitions.Add(new PluginDefinition("media.ffmpeg", "Media Converter", ConversionCategory.Media,
            Path.Combine(toolsRoot, "ffmpeg", "ffmpeg.exe"),
            BuildPairs(new[] { ".mp4", ".mp3", ".wav", ".flac", ".m4a", ".avi", ".mov", ".mkv", ".webm", ".aac" })));

        return definitions;
    }

    private static IReadOnlyCollection<string> BuildPairs(IReadOnlyCollection<string> extensions)
    {
        var pairs = new List<string>();
        foreach (var source in extensions)
        {
            foreach (var target in extensions)
            {
                if (!source.Equals(target, StringComparison.OrdinalIgnoreCase))
                {
                    pairs.Add($"{source}->{target}");
                }
            }
        }

        return pairs;
    }
}

public sealed class PluginDefinition
{
    public PluginDefinition(string pluginId, string displayName, ConversionCategory category, string executablePath, IReadOnlyCollection<string> supportedExtensions)
    {
        PluginId = pluginId;
        DisplayName = displayName;
        Category = category;
        ExecutablePath = executablePath;
        SupportedExtensions = supportedExtensions;
    }

    public string PluginId { get; }
    public string DisplayName { get; }
    public ConversionCategory Category { get; }
    public string ExecutablePath { get; }
    public IReadOnlyCollection<string> SupportedExtensions { get; }
}
