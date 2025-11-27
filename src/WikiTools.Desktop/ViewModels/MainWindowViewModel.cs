
namespace WikiTools.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ConverterViewModel ConverterViewModel { get; }

    public MainWindowViewModel()
    {
        ConverterViewModel = new ConverterViewModel();
    }
}
