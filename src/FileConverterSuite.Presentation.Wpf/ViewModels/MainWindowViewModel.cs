using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileConverterSuite.Application.Abstractions;
using FileConverterSuite.Domain.Entities;
using FileConverterSuite.Domain.ValueObjects;
using FileConverterSuite.Presentation.Wpf.Models;
using Microsoft.Win32;

namespace FileConverterSuite.Presentation.Wpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IConversionEngine _engine;

    [ObservableProperty]
    private double progress;

    public ObservableCollection<ConversionJobItem> Jobs { get; } = new();

    public MainWindowViewModel(IConversionEngine engine)
    {
        _engine = engine;
    }

    [RelayCommand]
    private void AddFiles()
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            foreach (var file in dialog.FileNames)
            {
                Jobs.Add(new ConversionJobItem
                {
                    InputPath = file,
                    OutputPath = Path.ChangeExtension(file, ".pdf")
                });
            }
        }
    }

    [RelayCommand]
    private async Task ConvertAllAsync()
    {
        if (Jobs.Count == 0)
        {
            return;
        }

        var index = 0;
        foreach (var job in Jobs)
        {
            job.Status = "Running";
            var inputExt = Path.GetExtension(job.InputPath);
            var outputExt = Path.GetExtension(job.OutputPath);

            var task = new ConversionTask
            {
                InputPath = job.InputPath,
                OutputPath = job.OutputPath,
                SourceFormat = new ConversionFormat(inputExt, inputExt),
                TargetFormat = new ConversionFormat(outputExt, outputExt)
            };

            await _engine.ConvertAsync(task, null, CancellationToken.None);
            job.Status = "Completed";
            index++;
            Progress = (double)index / Jobs.Count * 100d;
        }
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        var app = System.Windows.Application.Current;
        var currentTheme = app.Resources.MergedDictionaries.FirstOrDefault();
        var next = currentTheme?.Source.ToString().Contains("LightTheme") == true
            ? new Uri("Themes/DarkTheme.xaml", UriKind.Relative)
            : new Uri("Themes/LightTheme.xaml", UriKind.Relative);

        app.Resources.MergedDictionaries.Clear();
        app.Resources.MergedDictionaries.Add(new System.Windows.ResourceDictionary { Source = next });
    }

    public async Task HandleDroppedFilesAsync(IEnumerable<string> files)
    {
        foreach (var file in files.Where(File.Exists))
        {
            Jobs.Add(new ConversionJobItem
            {
                InputPath = file,
                OutputPath = Path.ChangeExtension(file, ".pdf")
            });
        }

        await Task.CompletedTask;
    }
}
