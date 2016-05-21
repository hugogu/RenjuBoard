namespace Renju.Infrastructure.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class WinRateWithPath
    {
        public WinRateWithPath(double rate, IEnumerable<IReadOnlyBoardPoint> drops = null)
        {
            WinRate = rate;
            Drops = drops.ToList();
        }

        public double WinRate { get; private set; }

        public IEnumerable<IReadOnlyBoardPoint> Drops { get; private set; }

        public static implicit operator WinRateWithPath(double rate)
        {
            return new WinRateWithPath(rate, new IReadOnlyBoardPoint[0]);
        }

        public IReadOnlyBoardPoint FindFirstDropOfSide(Side side)
        {
            return Drops.FirstOrDefault(p => p.Status.Value == side);
        }
    }
}
