using WikiTool.Desktop.Services;

namespace WikiTool.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public WikiBrowserViewModel WikiBrowserViewModel { get; }

    public MainWindowViewModel()
    {
        var folderPickerService = new FolderPickerService();
        WikiBrowserViewModel = new WikiBrowserViewModel(folderPickerService);
    }
}
