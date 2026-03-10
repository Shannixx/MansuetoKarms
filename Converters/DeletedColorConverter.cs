using System.Globalization;

namespace MansuetoKarms.Converters
{
    public class DeletedColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isDeleted && isDeleted)
                return Color.FromArgb("#FFF8E1");
            return Colors.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
