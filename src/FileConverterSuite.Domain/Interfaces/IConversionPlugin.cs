using FileConverterSuite.Domain.Entities;
using FileConverterSuite.Domain.Enums;

namespace FileConverterSuite.Domain.Interfaces;

public interface IConversionPlugin
{
    string PluginId { get; }
    string DisplayName { get; }
    ConversionCategory Category { get; }
    IReadOnlyCollection<string> SupportedExtensions { get; }
    bool CanHandle(ConversionTask task);
    Task ExecuteAsync(ConversionTask task, CancellationToken cancellationToken);
}
