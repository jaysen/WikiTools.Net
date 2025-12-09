using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WikiTool.Desktop.Models;

namespace WikiTool.Desktop.ViewModels;

public partial class CopyPagesViewModel : ViewModelBase
{
    private readonly ObservableCollection<WikiBrowserViewModel> _availableWikis;

    [ObservableProperty]
    private WikiBrowserViewModel? _sourceWiki;

    [ObservableProperty]
    private WikiBrowserViewModel? _targetWiki;

    [ObservableProperty]
    private ObservableCollection<SelectablePageNode> _selectablePages = [];

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private int _selectedPageCount;

    [ObservableProperty]
    private bool _preserveFolderStructure = true;

    [ObservableProperty]
    private bool _overwriteExisting;

    public CopyPagesViewModel(ObservableCollection<WikiBrowserViewModel> availableWikis)
    {
        _availableWikis = availableWikis;
    }

    public IEnumerable<WikiBrowserViewModel> AvailableSourceWikis =>
        _availableWikis.Where(w => w.HasWikiLoaded);

    public IEnumerable<WikiBrowserViewModel> AvailableTargetWikis =>
        _availableWikis.Where(w => w.HasWikiLoaded && w != SourceWiki);

    partial void OnSourceWikiChanged(WikiBrowserViewModel? value)
    {
        if (value != null)
        {
            LoadSelectablePages();
        }
        else
        {
            SelectablePages.Clear();
        }
        OnPropertyChanged(nameof(AvailableTargetWikis));
    }

    partial void OnTargetWikiChanged(WikiBrowserViewModel? value)
    {
        UpdateStatusMessage();
    }

    private void LoadSelectablePages()
    {
        if (SourceWiki == null || string.IsNullOrEmpty(SourceWiki.WikiRootPath))
        {
            SelectablePages.Clear();
            return;
        }

        var nodes = new List<SelectablePageNode>();
        BuildSelectableTree(SourceWiki.FolderTree, nodes);
        SelectablePages = new ObservableCollection<SelectablePageNode>(nodes);
        UpdateSelectedPageCount();
    }

    private void BuildSelectableTree(IEnumerable<FolderTreeNode> folderNodes, ICollection<SelectablePageNode> result, string relativePath = "")
    {
        foreach (var node in folderNodes)
        {
            var selectableNode = new SelectablePageNode(node, relativePath);
            selectableNode.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectablePageNode.IsSelected))
                {
                    UpdateSelectedPageCount();
                }
            };

            if (node.IsFolder && node.Children.Count > 0)
            {
                var newRelativePath = string.IsNullOrEmpty(relativePath)
                    ? node.Name
                    : Path.Combine(relativePath, node.Name);
                BuildSelectableTree(node.Children, selectableNode.Children, newRelativePath);
            }

            result.Add(selectableNode);
        }
    }

    private void UpdateSelectedPageCount()
    {
        SelectedPageCount = CountSelectedPages(SelectablePages);
        UpdateStatusMessage();
    }

    private int CountSelectedPages(IEnumerable<SelectablePageNode> nodes)
    {
        int count = 0;
        foreach (var node in nodes)
        {
            if (node.IsSelected && !node.IsFolder)
            {
                count++;
            }
            count += CountSelectedPages(node.Children);
        }
        return count;
    }

    private void UpdateStatusMessage()
    {
        if (SourceWiki == null)
        {
            StatusMessage = "Select a source wiki";
        }
        else if (TargetWiki == null)
        {
            StatusMessage = $"{SelectedPageCount} pages selected. Select a target wiki.";
        }
        else
        {
            StatusMessage = $"Ready to copy {SelectedPageCount} pages from '{SourceWiki.TabTitle}' to '{TargetWiki.TabTitle}'";
        }
    }

    [RelayCommand]
    private void SelectAllPages()
    {
        SetAllPagesSelected(SelectablePages, true);
        UpdateSelectedPageCount();
    }

    [RelayCommand]
    private void DeselectAllPages()
    {
        SetAllPagesSelected(SelectablePages, false);
        UpdateSelectedPageCount();
    }

    private void SetAllPagesSelected(IEnumerable<SelectablePageNode> nodes, bool isSelected)
    {
        foreach (var node in nodes)
        {
            node.IsSelected = isSelected;
            SetAllPagesSelected(node.Children, isSelected);
        }
    }

    [RelayCommand]
    private async Task CopyPagesAsync()
    {
        if (SourceWiki == null || TargetWiki == null || SelectedPageCount == 0)
        {
            return;
        }

        IsProcessing = true;
        StatusMessage = "Copying pages...";

        try
        {
            var selectedPages = GetSelectedPages(SelectablePages).ToList();
            int copiedCount = 0;

            foreach (var page in selectedPages)
            {
                await CopyPageAsync(page);
                copiedCount++;
                StatusMessage = $"Copied {copiedCount}/{selectedPages.Count} pages...";
            }

            StatusMessage = $"Successfully copied {copiedCount} pages!";

            // Refresh target wiki to show new files
            await TargetWiki.LoadWikiFolderAsync(TargetWiki.WikiRootPath);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error copying pages: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private IEnumerable<SelectablePageNode> GetSelectedPages(IEnumerable<SelectablePageNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.IsSelected && !node.IsFolder)
            {
                yield return node;
            }

            foreach (var child in GetSelectedPages(node.Children))
            {
                yield return child;
            }
        }
    }

    private async Task CopyPageAsync(SelectablePageNode page)
    {
        if (SourceWiki == null || TargetWiki == null)
        {
            return;
        }

        var sourceFile = page.FolderTreeNode.FullPath;
        string targetFile;

        if (PreserveFolderStructure)
        {
            // Preserve folder structure
            var targetFolder = Path.Combine(TargetWiki.WikiRootPath, page.RelativePath);
            Directory.CreateDirectory(targetFolder);
            targetFile = Path.Combine(targetFolder, page.FolderTreeNode.Name);
        }
        else
        {
            // Copy to root of target wiki
            targetFile = Path.Combine(TargetWiki.WikiRootPath, page.FolderTreeNode.Name);
        }

        if (File.Exists(targetFile) && !OverwriteExisting)
        {
            // Skip if file exists and overwrite is disabled
            return;
        }

        await Task.Run(() => File.Copy(sourceFile, targetFile, OverwriteExisting));
    }
}

public partial class SelectablePageNode : ObservableObject
{
    public FolderTreeNode FolderTreeNode { get; }
    public string RelativePath { get; }

    [ObservableProperty]
    private bool _isSelected;

    public ObservableCollection<SelectablePageNode> Children { get; } = [];

    public string Name => FolderTreeNode.Name;
    public bool IsFolder => FolderTreeNode.IsFolder;

    public SelectablePageNode(FolderTreeNode node, string relativePath)
    {
        FolderTreeNode = node;
        RelativePath = relativePath;
    }
}
