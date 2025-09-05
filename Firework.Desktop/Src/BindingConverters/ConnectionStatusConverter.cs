using System;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace Firework.Desktop.BindingConverters;

public class ConnectionStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isConnected)
        {
            return isConnected ? SymbolRegular.CheckmarkCircle24 : SymbolRegular.Warning24;
        }
        
        return SymbolRegular.Warning24;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

