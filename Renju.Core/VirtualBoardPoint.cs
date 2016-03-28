namespace Renju.Core
{
    public class VirtualBoardPoint : IReadOnlyBoardPoint
    {
        private readonly IReadOnlyBoardPoint _originalPoint;
        private readonly Side _side;

        public VirtualBoardPoint(IReadOnlyBoardPoint originalPoint, Side side)
        {
            _side = side;
            _originalPoint = originalPoint;
        }

        public int? Index
        {
            get { return _originalPoint.Index; }
        }

        public BoardPosition Position
        {
            get { return _originalPoint.Position; }
        }

        public Side? Status
        {
            get { return _side; }
        }

        public int Weight
        {
            get { return _originalPoint.Weight; }
            set { _originalPoint.Weight = value; }
        }
    }
}
