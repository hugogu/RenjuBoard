namespace Renju.Infrastructure.Model
{
    public class DropResult
    {
        public static DropResult InvalidDrop { get; private set; } = new DropResult { HasAnySideWon = false };

        public bool HasAnySideWon { get; private set; }

        public Side? WonSide { get; private set; }

        public Side? ExpectedNextSide { get; private set; }

        public static DropResult Win(Side side)
        {
            return new DropResult
            {
                HasAnySideWon = true,
                WonSide = side,
                ExpectedNextSide = null
            };
        }

        public static DropResult NoWin(Side nextSide)
        {
            return new DropResult
            {
                HasAnySideWon = false,
                ExpectedNextSide = nextSide,
            };
        }
    }
}
