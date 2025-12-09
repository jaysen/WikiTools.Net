using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using WikiTool.Desktop.ViewModels;

namespace WikiTool.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TabTitle_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is TextBlock textBlock && textBlock.DataContext is WikiBrowserViewModel viewModel)
        {
            viewModel.StartEditingTabTitleCommand.Execute(null);

            // Focus the TextBox after it becomes visible
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (textBlock.Parent is Panel panel)
                {
                    var textBox = panel.Children.OfType<TextBox>().FirstOrDefault();
                    if (textBox != null)
                    {
                        textBox.Focus();
                        textBox.SelectAll();
                    }
                }
            }, Avalonia.Threading.DispatcherPriority.Background);
        }
    }

    private void TabTitleEdit_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.DataContext is WikiBrowserViewModel viewModel)
        {
            viewModel.FinishEditingTabTitleCommand.Execute(null);
        }
    }

    private void TabTitleEdit_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is TextBox textBox && textBox.DataContext is WikiBrowserViewModel viewModel)
        {
            if (e.Key == Key.Enter)
            {
                viewModel.FinishEditingTabTitleCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                // Reset to original title and exit edit mode
                viewModel.FinishEditingTabTitleCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}