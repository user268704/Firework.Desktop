using System;
using System.Globalization;
using System.Windows.Data;

namespace Firework.Desktop.BindingConverters;

public class EmptyStringConnectionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && string.IsNullOrEmpty(str))
        {
            return parameter ?? "ожидание подключения";
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}