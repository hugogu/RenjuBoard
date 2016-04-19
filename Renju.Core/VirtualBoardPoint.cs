using System;
using System.Diagnostics;
using Renju.Infrastructure;
using Renju.Infrastructure.Model;

namespace Renju.Core
{
    [DebuggerDisplay("[{Index}-({Position.X},{Position.Y}):{Status}]")]
    public class VirtualBoardPoint : IReadOnlyBoardPoint
    {
        private readonly IReadOnlyBoardPoint _originalPoint;
        private readonly Side _side;
        private readonly int _index;

        public VirtualBoardPoint(IReadOnlyBoardPoint originalPoint, Side side, int index)
        {
            _side = side;
            _index = index;
            _originalPoint = originalPoint;
        }

        public int? Index
        {
            get { return _index; }
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

        public bool RequiresReevaluateWeight
        {
            get { return false; }
            set { /* Noop */ }
        }

        public override string ToString()
        {
            return String.Format("{1}{0}", Position, Status.GetLiternalPresentation());
        }
    }
}
