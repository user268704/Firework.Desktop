using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Firework.Desktop.BindingConverters;

public class ConnectionStatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isConnected)
        {
            return isConnected ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Orange);
        }
        
        return new SolidColorBrush(Colors.Orange);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

