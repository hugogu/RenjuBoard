namespace Renju.Core
{
    public struct DropResult
    {
        public static DropResult Win(Side side)
        {
            return new DropResult()
            {
                HasAnySideWon = true,
                WonSide = side,
                ExpectedNextSide = null
            };
        }

        public static DropResult NoWin(Side nextSide)
        {
            return new DropResult()
            {
                HasAnySideWon = false,
                ExpectedNextSide = nextSide,
            };
        }

        public static DropResult AlreadyDropped(Side nextSide)
        {
            return new DropResult()
            {
                HasAnySideWon = false,
                ExpectedNextSide = nextSide,
            };
        }

        public bool HasAnySideWon { get; private set; }

        public Side? WonSide { get; private set; }

        public Side? ExpectedNextSide { get; private set; }
    }
}
