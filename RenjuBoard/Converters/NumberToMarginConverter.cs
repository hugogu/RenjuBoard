using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RenjuBoard.Converters
{
    public class NumberToMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var left = System.Convert.ToDouble(values[0]);
            var top = values.Length > 1 ? System.Convert.ToDouble(values[1]) : 0;
            var right = values.Length > 2 ? System.Convert.ToDouble(values[2]) : 0;
            var bottom = values.Length > 3 ? System.Convert.ToDouble(values[3]) : 0;

            return new Thickness(left, top, right, bottom);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var thickness = (Thickness)value;

            return new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom };
        }
    }
}
