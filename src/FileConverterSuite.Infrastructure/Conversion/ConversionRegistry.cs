using FileConverterSuite.Domain.Interfaces;

namespace FileConverterSuite.Infrastructure.Conversion;

public sealed class ConversionRegistry : IConversionRegistry
{
    private readonly List<IConversionPlugin> _plugins = new();

    public IReadOnlyCollection<IConversionPlugin> Plugins => _plugins.AsReadOnly();

    public void Register(IConversionPlugin plugin)
    {
        if (_plugins.Any(p => p.PluginId.Equals(plugin.PluginId, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Plugin {plugin.PluginId} already registered.");
        }

        _plugins.Add(plugin);
    }

    public IConversionPlugin Resolve(string sourceExtension, string targetExtension)
    {
        var normalized = Normalize(sourceExtension, targetExtension);
        var plugin = _plugins.FirstOrDefault(p => p.SupportedExtensions.Contains(normalized));
        return plugin ?? throw new NotSupportedException($"No plugin found for {normalized}.");
    }

    private static string Normalize(string sourceExtension, string targetExtension)
    {
        var src = sourceExtension.StartsWith(".") ? sourceExtension.ToLowerInvariant() : "." + sourceExtension.ToLowerInvariant();
        var dst = targetExtension.StartsWith(".") ? targetExtension.ToLowerInvariant() : "." + targetExtension.ToLowerInvariant();
        return $"{src}->{dst}";
    }
}
