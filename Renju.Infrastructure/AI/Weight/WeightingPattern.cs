namespace Renju.Infrastructure.AI.Weight
{
    using System;
    using System.Diagnostics.Contracts;

    public class WeightingPattern
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

        public override string ToString()
        {
            return string.Format("WeightPattern[({1})->{0}]", Pattern, OriginPattern);
        }

        public override int GetHashCode()
        {
            return Pattern.GetHashCode();
        }
    }
}
