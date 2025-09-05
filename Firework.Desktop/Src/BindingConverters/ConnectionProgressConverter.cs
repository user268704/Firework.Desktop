using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Firework.Desktop.BindingConverters;

public class ConnectionProgressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isConnected)
        {
            // Показываем прогресс-бар только когда НЕ подключены (ожидание подключения)
            return isConnected ? Visibility.Collapsed : Visibility.Visible;
        }
        
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

