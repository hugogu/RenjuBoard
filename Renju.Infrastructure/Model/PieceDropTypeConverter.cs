using System;
using System.ComponentModel;
using System.Globalization;

namespace Renju.Infrastructure.Model
{
    public class PieceDropTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (value is string)
            {
                var values = (value as string).Split(' ');
                if (values.Length != 3)
                    throw new ArgumentException("String form of PeiceDrop must have 3 segments.");
                return new PieceDrop(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), (Side)Enum.Parse(typeof(Side), values[2]));
            }
            throw new InvalidOperationException(String.Format("Can't convert {0} to PieceDrop.", value.GetType()));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;

            if (destinationType == typeof(string))
            {
                var drop = value as PieceDrop;
                return String.Format("{0} {1} {2}", drop.X, drop.Y, drop.Side);
            }

            throw new InvalidOperationException("Can't convert PieceDrop to type " + destinationType.FullName);
        }
    }
}
