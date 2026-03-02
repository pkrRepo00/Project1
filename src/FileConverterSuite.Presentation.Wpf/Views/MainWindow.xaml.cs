using System.Windows;

namespace FileConverterSuite.Presentation.Wpf.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.ServicesProvider.GetService(typeof(ViewModels.MainWindowViewModel));
        DragOver += (_, e) => e.Effects = DragDropEffects.Copy;
        Drop += async (_, e) =>
        {
            if (DataContext is ViewModels.MainWindowViewModel vm && e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                await vm.HandleDroppedFilesAsync(files);
            }
        };
    }
}
