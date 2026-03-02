using FileConverterSuite.Application.Abstractions;
using FileConverterSuite.Application.Options;
using FileConverterSuite.Domain.Entities;
using FileConverterSuite.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileConverterSuite.Application.Services;

public sealed class ConversionEngine : IConversionEngine
{
    private readonly IConversionRegistry _registry;
    private readonly IFileValidationService _validationService;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<ConversionEngine> _logger;

    public ConversionEngine(
        IConversionRegistry registry,
        IFileValidationService validationService,
        IOptions<ConversionSettings> settings,
        ILogger<ConversionEngine> logger)
    {
        _registry = registry;
        _validationService = validationService;
        _logger = logger;
        _semaphore = new SemaphoreSlim(settings.Value.MaxParallelConversions, settings.Value.MaxParallelConversions);
    }

    public async Task ConvertAsync(ConversionTask task, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var plugin = _registry.Resolve(task.SourceFormat.Extension, task.TargetFormat.Extension);
            _validationService.Validate(task.InputPath, task.OutputPath, plugin.SupportedExtensions);
            progress?.Report(0.1);

            await plugin.ExecuteAsync(task, cancellationToken).ConfigureAwait(false);

            progress?.Report(1.0);
            _logger.LogInformation("Conversion {TaskId} completed using {PluginId}", task.TaskId, plugin.PluginId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Conversion {TaskId} failed", task.TaskId);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
