namespace FileConverterSuite.Application.Options;

public sealed class ConversionSettings
{
    public int MaxParallelConversions { get; set; } = Math.Max(2, Environment.ProcessorCount / 2);
    public long MaxInputSizeMb { get; set; } = 4096;
    public bool LocalOnlyProcessing { get; set; } = true;
    public string DefaultOutputDirectory { get; set; } = string.Empty;
}
