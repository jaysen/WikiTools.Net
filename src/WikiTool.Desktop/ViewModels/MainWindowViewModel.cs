using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WikiTool.Desktop.Services;
using WikiTool.Desktop.Views;

namespace WikiTool.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFolderPickerService _folderPickerService;

    [ObservableProperty]
    private ObservableCollection<WikiBrowserViewModel> _wikiTabs = [];

    [ObservableProperty]
    private WikiBrowserViewModel? _selectedTab;

    public MainWindowViewModel()
    {
        _folderPickerService = new FolderPickerService();

        // Create initial tab
        OpenNewWikiTab();
    }

    public int OpenTabsCount => WikiTabs.Count;
    public bool HasOpenTabs => WikiTabs.Count > 0;

    partial void OnWikiTabsChanged(ObservableCollection<WikiBrowserViewModel> value)
    {
        OnPropertyChanged(nameof(OpenTabsCount));
        OnPropertyChanged(nameof(HasOpenTabs));
    }

    [RelayCommand]
    private void OpenNewWikiTab()
    {
        var newTab = new WikiBrowserViewModel(_folderPickerService);
        WikiTabs.Add(newTab);
        SelectedTab = newTab;
        OnPropertyChanged(nameof(OpenTabsCount));
        OnPropertyChanged(nameof(HasOpenTabs));
    }

    [RelayCommand]
    private void CloseTab(WikiBrowserViewModel? tab)
    {
        if (tab == null) return;

        var index = WikiTabs.IndexOf(tab);
        WikiTabs.Remove(tab);

        // Select adjacent tab or create new one if no tabs left
        if (WikiTabs.Count == 0)
        {
            OpenNewWikiTab();
        }
        else if (index >= WikiTabs.Count)
        {
            SelectedTab = WikiTabs[WikiTabs.Count - 1];
        }
        else
        {
            SelectedTab = WikiTabs[index];
        }

        OnPropertyChanged(nameof(OpenTabsCount));
        OnPropertyChanged(nameof(HasOpenTabs));
    }

    [RelayCommand]
    private async Task OpenCopyPagesWindowAsync()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = desktop.MainWindow;
            if (mainWindow != null)
            {
                var viewModel = new CopyPagesViewModel(
                    WikiTabs,
                    _folderPickerService,
                    SelectedTab,
                    OpenFolderAsNewTab);
                var copyWindow = new CopyPagesWindow(viewModel)
                {
                    Icon = mainWindow.Icon
                };
                await copyWindow.ShowDialog(mainWindow);
            }
        }
    }

    private async void OpenFolderAsNewTab(string folderPath)
    {
        var newTab = new WikiBrowserViewModel(_folderPickerService);
        WikiTabs.Add(newTab);
        SelectedTab = newTab;
        await newTab.LoadWikiFolderAsync(folderPath);
        OnPropertyChanged(nameof(OpenTabsCount));
        OnPropertyChanged(nameof(HasOpenTabs));
    }
}
