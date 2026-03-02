using System.Diagnostics;
using FileConverterSuite.Domain.Entities;
using FileConverterSuite.Domain.Enums;
using FileConverterSuite.Domain.Interfaces;

namespace FileConverterSuite.Infrastructure.Conversion;

public sealed class ExternalToolPlugin : IConversionPlugin
{
    private readonly Func<ConversionTask, string> _argumentsFactory;
    private readonly string _executablePath;

    public ExternalToolPlugin(
        string pluginId,
        string displayName,
        ConversionCategory category,
        string executablePath,
        IEnumerable<string> supportedExtensions,
        Func<ConversionTask, string> argumentsFactory)
    {
        PluginId = pluginId;
        DisplayName = displayName;
        Category = category;
        _executablePath = executablePath;
        SupportedExtensions = supportedExtensions.ToArray();
        _argumentsFactory = argumentsFactory;
    }

    public string PluginId { get; }
    public string DisplayName { get; }
    public ConversionCategory Category { get; }
    public IReadOnlyCollection<string> SupportedExtensions { get; }

    public bool CanHandle(ConversionTask task)
    {
        var key = $"{task.SourceFormat.Extension}->{task.TargetFormat.Extension}";
        return SupportedExtensions.Contains(key, StringComparer.OrdinalIgnoreCase);
    }

    public async Task ExecuteAsync(ConversionTask task, CancellationToken cancellationToken)
    {
        if (!File.Exists(_executablePath))
        {
            throw new FileNotFoundException($"Required conversion executable not found at {_executablePath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            Arguments = _argumentsFactory(task),
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start conversion process.");
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

        if (process.ExitCode != 0)
        {
            var errorText = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
            throw new InvalidOperationException($"Plugin {PluginId} failed with exit code {process.ExitCode}: {errorText}");
        }
    }
}
