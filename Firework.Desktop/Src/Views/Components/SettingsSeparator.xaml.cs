using System.Windows.Controls;

namespace Firework.Desktop.Views.Components;

public partial class SettingsSeparator : UserControl
{
    private string _title;

    public SettingsSeparator()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => _title;
        set
        {
            TitleBlock.Text = value;
            _title = value;
        }
    }
}