using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WikiTools.Converters;
using WikiTools.Desktop.Services;

namespace WikiTools.Desktop.ViewModels;

public partial class ConverterViewModel : ViewModelBase
{
    private readonly IFolderPickerService _folderPickerService;

    public ConverterViewModel(IFolderPickerService folderPickerService)
    {
        _folderPickerService = folderPickerService;
    }

    // Observable Properties
    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _destinationPath = string.Empty;

    [ObservableProperty]
    private bool _convertCategoryTags = false;

    [ObservableProperty]
    private bool _isConverting = false;

    [ObservableProperty]
    private bool _hasConversionResults = false;

    [ObservableProperty]
    private string _statusMessage = "Ready to convert";

    [ObservableProperty]
    private string? _sourcePathError;

    [ObservableProperty]
    private string? _destinationPathError;

    // Computed Property
    public bool CanConvert => !string.IsNullOrWhiteSpace(SourcePath)
                           && !string.IsNullOrWhiteSpace(DestinationPath)
                           && !IsConverting
                           && string.IsNullOrEmpty(SourcePathError)
                           && string.IsNullOrEmpty(DestinationPathError);

    // Validation Partial Methods
    partial void OnSourcePathChanged(string value)
    {
        ValidateSourcePath(value);
        ConvertCommand.NotifyCanExecuteChanged();
    }

    partial void OnDestinationPathChanged(string value)
    {
        ValidateDestinationPath(value);
        ConvertCommand.NotifyCanExecuteChanged();
    }

    private void ValidateSourcePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            SourcePathError = null;
            return;
        }

        SourcePathError = Directory.Exists(path)
            ? null
            : "Source directory does not exist";
    }

    private void ValidateDestinationPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            DestinationPathError = null;
            return;
        }

        var parentDir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
        {
            DestinationPathError = "Parent directory does not exist";
        }
        else
        {
            DestinationPathError = null;
        }
    }

    // Commands
    [RelayCommand]
    private async Task BrowseSourceAsync()
    {
        var path = await _folderPickerService.PickFolderAsync("Select WikidPad Source Folder");
        if (!string.IsNullOrEmpty(path))
        {
            SourcePath = path;
        }
    }

    [RelayCommand]
    private async Task BrowseDestinationAsync()
    {
        var path = await _folderPickerService.PickFolderAsync("Select Obsidian Destination Folder");
        if (!string.IsNullOrEmpty(path))
        {
            DestinationPath = path;
        }
    }

    [RelayCommand(CanExecute = nameof(CanConvert))]
    private async Task ConvertAsync()
    {
        try
        {
            IsConverting = true;
            HasConversionResults = false;
            StatusMessage = "Starting conversion...";

            await Task.Run(() =>
            {
                var converter = new WikidPadToObsidianConverter(SourcePath, DestinationPath){};
                converter.ConvertAll();
            });

            StatusMessage = "Conversion completed successfully!";
            HasConversionResults = true;
        }
        catch (DirectoryNotFoundException)
        {
            StatusMessage = "Conversion failed - directory not found";
            HasConversionResults = true;
        }
        catch (Exception)
        {
            StatusMessage = "Conversion failed - unexpected error";
            HasConversionResults = true;
        }
        finally
        {
            IsConverting = false;
        }
    }

}
