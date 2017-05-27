namespace Renju.Infrastructure.AI.Weight
{
    using System;
    using System.Diagnostics.Contracts;

    public class WeightingPattern : IEquatable<WeightingPattern>
    {
        public WeightingPattern(string pattern, string originRegularPattern)
        {
            Contract.Assert(pattern != null);
            Contract.Assert(originRegularPattern != null);
            Pattern = pattern;
            OriginPattern = originRegularPattern;
        }

        public string Pattern { get; private set; }

        public string OriginPattern { get; private set; }

        public static implicit operator WeightingPattern(string pattern)
        {
            return new WeightingPattern(pattern, String.Empty);
        }

        public static implicit operator String(WeightingPattern pattern)
        {
            return pattern.Pattern;
        }

        public override string ToString()
        {
            return string.Format("WeightPattern[({1})->{0}]", Pattern, OriginPattern);
        }

        public override int GetHashCode()
        {
            return Pattern.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WeightingPattern);
        }

        public bool Equals(WeightingPattern other)
        {
            return other == null ? false : other.Pattern.Equals(Pattern);
        }
    }
}
