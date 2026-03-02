using FileConverterSuite.Application.Abstractions;
using FileConverterSuite.Application.Options;
using FileConverterSuite.Application.Services;
using FileConverterSuite.Domain.Interfaces;
using FileConverterSuite.Infrastructure.Configuration;
using FileConverterSuite.Infrastructure.Conversion;
using FileConverterSuite.Infrastructure.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FileConverterSuite.Infrastructure;

public static class Services
{
    public static IServiceCollection AddFileConverterSuite(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConversionSettings>(configuration.GetSection("ConversionSettings"));
        services.Configure<ToolingOptions>(configuration.GetSection("Tooling"));

        services.AddSingleton<IConversionRegistry, ConversionRegistry>();
        services.AddSingleton<IConversionQueue, InMemoryConversionQueue>();
        services.AddSingleton<IFileValidationService, FileValidationService>();
        services.AddSingleton<IConversionEngine, ConversionEngine>();
        services.AddSingleton<PluginLoader>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app-.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddLogging(builder => builder.AddSerilog(Log.Logger, dispose: true));
        return services;
    }
}
