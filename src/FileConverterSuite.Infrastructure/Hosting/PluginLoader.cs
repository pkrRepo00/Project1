using FileConverterSuite.Domain.Entities;
using FileConverterSuite.Domain.Interfaces;
using FileConverterSuite.Infrastructure.Configuration;
using FileConverterSuite.Infrastructure.Conversion;
using Microsoft.Extensions.Options;

namespace FileConverterSuite.Infrastructure.Hosting;

public sealed class PluginLoader
{
    private readonly IConversionRegistry _registry;
    private readonly ToolingOptions _tooling;

    public PluginLoader(IConversionRegistry registry, IOptions<ToolingOptions> tooling)
    {
        _registry = registry;
        _tooling = tooling.Value;
    }

    public void Load()
    {
        foreach (var definition in ConversionCatalog.BuildDefinitions(_tooling.ToolsRoot))
        {
            var plugin = new ExternalToolPlugin(
                definition.PluginId,
                definition.DisplayName,
                definition.Category,
                definition.ExecutablePath,
                definition.SupportedExtensions,
                task => BuildArguments(definition.PluginId, task));

            _registry.Register(plugin);
        }
    }

    private static string BuildArguments(string pluginId, ConversionTask task)
    {
        if (pluginId.StartsWith("media", StringComparison.OrdinalIgnoreCase))
        {
            return $"-y -i \"{task.InputPath}\" \"{task.OutputPath}\"";
        }

        if (pluginId.StartsWith("archive", StringComparison.OrdinalIgnoreCase))
        {
            var targetExt = Path.GetExtension(task.OutputPath).ToLowerInvariant();
            return targetExt switch
            {
                ".zip" or ".7z" => $"a \"{task.OutputPath}\" \"{task.InputPath}\"",
                _ => $"x \"{task.InputPath}\" -o\"{task.OutputPath}\" -y"
            };
        }

        if (pluginId.StartsWith("document", StringComparison.OrdinalIgnoreCase))
        {
            var outDir = Path.GetDirectoryName(task.OutputPath) ?? ".";
            var format = Path.GetExtension(task.OutputPath).TrimStart('.');
            return $"--headless --convert-to {format} --outdir \"{outDir}\" \"{task.InputPath}\"";
        }

        return $"\"{task.InputPath}\" \"{task.OutputPath}\"";
    }
}
