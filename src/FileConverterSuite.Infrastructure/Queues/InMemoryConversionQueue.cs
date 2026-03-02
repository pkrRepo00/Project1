using System.Threading.Channels;
using FileConverterSuite.Application.Abstractions;
using FileConverterSuite.Domain.Entities;

namespace FileConverterSuite.Infrastructure.Queues;

public sealed class InMemoryConversionQueue : IConversionQueue
{
    private readonly Channel<ConversionTask> _channel = Channel.CreateUnbounded<ConversionTask>(
        new UnboundedChannelOptions { SingleReader = false, SingleWriter = false, AllowSynchronousContinuations = false });

    public ValueTask QueueAsync(ConversionTask task, CancellationToken cancellationToken)
        => _channel.Writer.WriteAsync(task, cancellationToken);

    public IAsyncEnumerable<ConversionTask> DequeueAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}
