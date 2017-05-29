namespace Renju.Infrastructure.AI.Weight
{
    public class WeightingInfo
    {
        public WeightingInfo(string pattern, double weight)
        {
            OriginPattern = pattern;
            Weight = weight;
        }

        public string OriginPattern { get; internal set; }

        public double Weight { get; internal set; }
    }
}
