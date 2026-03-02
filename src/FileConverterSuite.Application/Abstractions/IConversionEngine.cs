using FileConverterSuite.Domain.Entities;

namespace FileConverterSuite.Application.Abstractions;

public interface IConversionEngine
{
    Task ConvertAsync(ConversionTask task, IProgress<double>? progress, CancellationToken cancellationToken);
}
