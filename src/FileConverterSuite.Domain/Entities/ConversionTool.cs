namespace FileConverterSuite.Domain.Entities;

public sealed class ConversionTool
{
    public required string Name { get; init; }
    public required string ExecutablePath { get; init; }
    public required string CommercialLicense { get; init; }
}
