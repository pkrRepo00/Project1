namespace FileConverterSuite.Presentation.Wpf.Models;

public sealed class ConversionJobItem
{
    public required string InputPath { get; init; }
    public required string OutputPath { get; init; }
    public string Status { get; set; } = "Queued";
}
