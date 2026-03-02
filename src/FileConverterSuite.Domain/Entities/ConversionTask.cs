using FileConverterSuite.Domain.ValueObjects;

namespace FileConverterSuite.Domain.Entities;

public sealed class ConversionTask
{
    public Guid TaskId { get; init; } = Guid.NewGuid();
    public required string InputPath { get; init; }
    public required string OutputPath { get; init; }
    public required ConversionFormat SourceFormat { get; init; }
    public required ConversionFormat TargetFormat { get; init; }
    public IReadOnlyDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();
}
