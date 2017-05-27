namespace Renju.Infrastructure.AI.Weight
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class WeightingRule
    {
        public WeightingRule(string regularPattern, int priority)
        {
            Contract.Assert(regularPattern != null);
            Priority = priority;
            var patternWithWeight = regularPattern.Split(':');
            RegularPatterns = patternWithWeight[0].Split(',');
            if (patternWithWeight.Length > 1)
            {
                Weight = Convert.ToInt32(patternWithWeight[1]);
            }
            foreach (var pattern in RegularPatterns)
            {
                Contract.Assert(pattern.Contains("."), "A pattern should contain a '.'");
            }
        }

        public IEnumerable<string> RegularPatterns { get; private set; }

        public int? Weight { get; private set; }

        public int Priority { get; private set; }

        public IEnumerable<WeightingPattern> GenerateAllPatterns(int maxPatternLength)
        {
            return from regularPattern in RegularPatterns
                   from pattern in GeneratePatterns(regularPattern, maxPatternLength)
                   where !(pattern.Contains("+++++") || pattern.Contains("-----"))
                   select new WeightingPattern(pattern, regularPattern);
        }

        [Pure]
        public static IEnumerable<string> GeneratePatterns(string pattern, int maxLength)
        {
            return Enumerable.Range(pattern.Length + 1, maxLength - pattern.Length)
                             .SelectMany(length => GeneratePatternsAtLength(pattern, length))
                             .Concat(new[] { pattern });
        }

        [Pure]
        public static IEnumerable<string> GeneratePatternsAtLength(string pattern, int length)
        {
            int fillerLength = length - pattern.Length;
            int insertIndex = Math.Min(fillerLength, Math.Max(0, length / 2 - pattern.IndexOf('.')));
            foreach (var filler in GenerateAllArrangement(fillerLength))
            {
                yield return filler.Insert(insertIndex, pattern);
            }
        }

        [Pure]
        public static IEnumerable<String> GenerateAllArrangement(int maxLength)
        {
            foreach (var primary in EnumeratePrimaryPatterns())
            {
                if (maxLength > 1)
                    foreach (var tail in GenerateAllArrangement(maxLength - 1))
                        yield return primary + tail;
                else
                    yield return primary;
            }
        }

        [Pure]
        public static IEnumerable<String> EnumeratePrimaryPatterns()
        {
            yield return "_";
            yield return "-";
            yield return "+";
        }
    }
}
