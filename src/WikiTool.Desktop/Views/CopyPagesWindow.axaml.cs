using Avalonia.Controls;
using Avalonia.Interactivity;
using WikiTool.Desktop.ViewModels;

namespace WikiTool.Desktop.Views;

public partial class CopyPagesWindow : Window
{
    public CopyPagesWindow()
    {
        InitializeComponent();
    }

    public CopyPagesWindow(CopyPagesViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
