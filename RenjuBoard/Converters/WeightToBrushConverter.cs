using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RenjuBoard.Converters
{
    [ValueConversion(typeof(double), typeof(Brush))]
    public class WeightToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromArgb((int)value > 5 ? (byte)255 : (byte)0, (byte) ((int)value | 0xFF), (byte)((int)value >> 8 * 10), 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
