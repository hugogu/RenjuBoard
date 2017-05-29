namespace Renju.Infrastructure.Model
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;

    public class PieceDropTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Debug.Assert(value != null);
            Debug.Assert(value is string);
            var values = (value as string).Split(' ');
            Debug.Assert(values.Length == 3, "String form of PeiceDrop must have 3 segments.");

            return new PieceDrop(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), (Side)Enum.Parse(typeof(Side), values[2]));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Debug.Assert(value != null);
            Debug.Assert(typeof(string).IsAssignableFrom(destinationType));
            var drop = value as PieceDrop;

            return String.Format("{0} {1} {2}", drop.X, drop.Y, drop.Side);
        }
    }
}
