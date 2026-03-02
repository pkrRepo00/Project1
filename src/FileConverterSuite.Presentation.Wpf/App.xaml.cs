using System.Windows;
using FileConverterSuite.Infrastructure;
using FileConverterSuite.Infrastructure.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileConverterSuite.Presentation.Wpf;

public partial class App : Application
{
    public static IServiceProvider ServicesProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection()
            .AddFileConverterSuite(configuration)
            .AddSingleton<ViewModels.MainWindowViewModel>()
            .BuildServiceProvider();

        ServicesProvider = services;
        services.GetRequiredService<PluginLoader>().Load();

        base.OnStartup(e);
    }
}
