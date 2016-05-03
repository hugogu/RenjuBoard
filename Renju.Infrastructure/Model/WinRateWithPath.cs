using System.Collections.Generic;
using System.Linq;

namespace Renju.Infrastructure.Model
{
    public class WinRateWithPath
    {
        public double WinRate { get; private set; }

        public IEnumerable<IReadOnlyBoardPoint> Drops { get; private set; }

        public WinRateWithPath(double rate, IEnumerable<IReadOnlyBoardPoint> drops = null)
        {
            WinRate = rate;
            Drops = drops;
        }

        public static implicit operator WinRateWithPath(double rate)
        {
            return new WinRateWithPath(rate);
        }

        public IReadOnlyBoardPoint FindFirstDropOfSide(Side side)
        {
            return Drops.First(p => p.Status.Value == side);
        }
    }
}
