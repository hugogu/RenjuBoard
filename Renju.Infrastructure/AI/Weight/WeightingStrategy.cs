namespace Renju.Infrastructure.AI.Weight
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Practices.Unity;
    using Model;
    using Model.Extensions;

    public class WeightingStrategy : IDropWeightEvaluator
    {
        private readonly IList<WeightingRule> rules = new List<WeightingRule>();
        private readonly ConcurrentDictionary<string, WeightingInfo> weightingPatterns = new ConcurrentDictionary<string, WeightingInfo>();
        private readonly Regex regularPatternRegex = new Regex("[+-._,]+");

        [InjectionConstructor]
        public WeightingStrategy(string weightPatternFile)
            : this(File.OpenRead(weightPatternFile))
        {
        }

        public WeightingStrategy(Stream stream)
        {
            LoadRulesFromStream(stream);
            GeneratePatterns();
        }

        public int PatternLength { get; private set; }

        public IDictionary<string, WeightingInfo> WeightingPatterns
        {
            get { return weightingPatterns; }
        }

        public IList<WeightingRule> Rules
        {
            get { return rules; }
        }

        public virtual void LoadRulesFromStream(Stream rulesStream)
        {
            var strategyReader = new StreamReader(rulesStream);
            int rulePriority = 0;
            while (!strategyReader.EndOfStream)
            {
                var line = strategyReader.ReadLine().Trim();
                if (line.StartsWith("#"))
                {
                    continue;
                }
                else if (line.StartsWith("maxlength"))
                {
                    PatternLength = Convert.ToInt32(line.Substring(10));
                }
                else if (regularPatternRegex.IsMatch(line))
                {
                    rules.Add(new WeightingRule(line, rulePriority++));
                }
            }
        }

        public virtual void GeneratePatterns()
        {
            foreach (var rule in rules.OrderBy(r => r.Priority))
            {
                foreach (var pattern in rule.GenerateAllPatterns(PatternLength))
                {
                    weightingPatterns.AddOrUpdate(pattern, 
                        key => new WeightingInfo(pattern.OriginPattern, rule.Weight ?? (rules.Count - rule.Priority) * 4),
                        (key, old) => old.OriginPattern.Length < pattern.OriginPattern.Length ?
                                new WeightingInfo(pattern.OriginPattern, rule.Weight ?? (rules.Count - rule.Priority) * 4) :
                                old);
                }
            }
        }

        public double CalculateWeight(IReadBoardState<IReadOnlyBoardPoint> board, IReadOnlyBoardPoint drop, Side dropSide)
        {
            return board.GetRowsFromPoint(drop, true)
                        .Where(l => l.Length <= PatternLength)
                        .Select(l => l.ResizeTo(PatternLength, drop.Position)
                                      .GetPatternStringOfSide(drop.Position, dropSide))
                        .Sum(p => FindWeightForPattern(p));
        }

        private double FindWeightForPattern(string pattern)
        {
            WeightingInfo weight = null;
            if (weightingPatterns.TryGetValue(pattern, out weight) ||
                weightingPatterns.TryGetValue(Reverse(pattern), out weight))
            {
                return weight.Weight;
            }
            Trace.WriteLine("Pattern {0} not found", pattern);

            return 0;
        }

        private string Reverse(string value)
        {
            return new string(value.Reverse().ToArray());
        }
    }
}
