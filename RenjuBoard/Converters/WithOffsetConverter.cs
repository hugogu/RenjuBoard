using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace RenjuBoard.Converters
{
    [ValueConversion(typeof(object), typeof(double))]
    public class WithOffsetConverter : IValueConverter
    {
        private readonly TypeConverter _doubleConverter = TypeDescriptor.GetConverter(typeof(double));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Double.NaN;

            var offSet = (double)_doubleConverter.ConvertFrom(parameter);
            if (_doubleConverter.CanConvertFrom(value.GetType()))
            {
                return (double)_doubleConverter.ConvertFrom(value) + offSet;
            }

            var valueConverter = TypeDescriptor.GetConverter(value);
            if (valueConverter.CanConvertTo(typeof(double)))
            {
                return (double)valueConverter.ConvertTo(value, typeof(double)) + offSet;
            }

            throw new ArgumentException(String.Format("Missing type converter from {0} to double.", value.GetType()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
