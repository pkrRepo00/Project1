namespace FileConverterSuite.Application.Abstractions;

public interface IFileValidationService
{
    void Validate(string inputPath, string outputPath, IReadOnlyCollection<string> supportedExtensions);
}
