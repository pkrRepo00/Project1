using FileConverterSuite.Application.Abstractions;

namespace FileConverterSuite.Infrastructure.Configuration;

public sealed class FileValidationService : IFileValidationService
{
    public void Validate(string inputPath, string outputPath, IReadOnlyCollection<string> supportedExtensions)
    {
        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException("Input file was not found.", inputPath);
        }

        var sourceExt = Path.GetExtension(inputPath).ToLowerInvariant();
        var targetExt = Path.GetExtension(outputPath).ToLowerInvariant();
        var key = $"{sourceExt}->{targetExt}";

        if (!supportedExtensions.Contains(key, StringComparer.OrdinalIgnoreCase))
        {
            throw new NotSupportedException($"Unsupported conversion pair {key}.");
        }

        var fileInfo = new FileInfo(inputPath);
        if (fileInfo.Length == 0)
        {
            throw new InvalidDataException("Input file appears to be empty or corrupted.");
        }
    }
}
