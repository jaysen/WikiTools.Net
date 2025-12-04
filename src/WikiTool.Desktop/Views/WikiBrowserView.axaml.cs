using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using WikiTool.Desktop.ViewModels;

namespace WikiTool.Desktop.Views;

public partial class WikiBrowserView : UserControl
{
    public WikiBrowserView()
    {
        InitializeComponent();
    }

    private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is WikiBrowserViewModel viewModel)
        {
            viewModel.CloseConverterDialogCommand.Execute(null);
        }
    }
}
