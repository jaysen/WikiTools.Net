using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WikiTools.Converters;

namespace WikiTools.Desktop.ViewModels;

public partial class ConverterViewModel : ViewModelBase
{
    // Observable Properties
    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _destinationPath = string.Empty;

    [ObservableProperty]
    private bool _convertCategoryTags = false;

}
