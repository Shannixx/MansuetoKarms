using System.Globalization;
using Microsoft.Maui.Controls;

namespace MansuetoKarms.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected && parameter is string colors)
            {
                var parts = colors.Split('|');
                if (parts.Length == 2)
                {
                    // Parse color from hex string
                    var colorString = isSelected ? parts[0] : parts[1];
                    return Color.FromArgb(colorString);
                }
            }
            return Colors.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}