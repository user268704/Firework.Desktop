using System.Windows.Controls;
using Firework.Desktop.ViewModel;
using Wpf.Ui.Controls;

namespace Firework.Desktop.Views.Pages;

public partial class SettingsPage : Page, INavigableView<SettingsViewModel>
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        DataContext = this;
        
        ViewModel = settingsViewModel;
    }

    public SettingsViewModel ViewModel { get; }
}