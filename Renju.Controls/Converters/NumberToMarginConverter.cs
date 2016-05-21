namespace Renju.Controls.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    public class NumberToMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => v == DependencyProperty.UnsetValue))
                return new Thickness();

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
