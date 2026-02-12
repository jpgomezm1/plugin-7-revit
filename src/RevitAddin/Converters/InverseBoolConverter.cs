using System.Globalization;
using System.Windows.Data;

namespace DocumentationGeneratorAI.RevitAddin.Converters;

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && parameter is string options)
        {
            var parts = options.Split('|');
            if (parts.Length == 2)
                return b ? parts[1] : parts[0];
        }

        if (value is bool boolVal)
            return !boolVal;

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return value;
    }
}
