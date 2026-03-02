using FileConverterSuite.Domain.Entities;

namespace FileConverterSuite.Application.Abstractions;

public interface IConversionQueue
{
    ValueTask QueueAsync(ConversionTask task, CancellationToken cancellationToken);
    IAsyncEnumerable<ConversionTask> DequeueAllAsync(CancellationToken cancellationToken);
}
