namespace Renju.Infrastructure.Model
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using Microsoft.Practices.Unity.Utility;

    public class PieceDropTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Guard.ArgumentNotNull(value, nameof(value));
            Guard.InstanceIsAssignable(typeof(string), value, nameof(value));
            var values = (value as string).Split(' ');
            if (values.Length != 3)
                throw new ArgumentException("String form of PeiceDrop must have 3 segments.");

            return new PieceDrop(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), (Side)Enum.Parse(typeof(Side), values[2]));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Guard.ArgumentNotNull(value, nameof(value));
            Guard.TypeIsAssignable(destinationType, typeof(string), nameof(destinationType));
            var drop = value as PieceDrop;

            return String.Format("{0} {1} {2}", drop.X, drop.Y, drop.Side);
        }
    }
}
