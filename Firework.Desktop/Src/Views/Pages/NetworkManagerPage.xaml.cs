using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Firework.Desktop.ViewModel;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace Firework.Desktop.Views.Pages;

public partial class NetworkManagerPage : Page, INavigableView<NetworkManagerViewModel>
{
    public NetworkManagerViewModel ViewModel { get; set; }

    public NetworkManagerPage(NetworkManagerViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    private void ClickHostNameCard(object sender, MouseButtonEventArgs e)
    {
        if (HostTextBox.Visibility == Visibility.Collapsed)
        {
            HostTextBlock.Visibility = Visibility.Collapsed;
            HostTextBox.Visibility = Visibility.Visible;
        }
        else
        {
            HostTextBlock.Visibility = Visibility.Visible;
            HostTextBox.Visibility = Visibility.Collapsed;
        }
    }

    private void SaveChangesHostButton_OnClick(object sender, RoutedEventArgs e)
    {

    }
}