using System;
using System.Linq;
using System.Threading.Tasks;
using Renju.Core;

namespace Renju.AI
{
    public class AIGamePlayer : IGamePlayer
    {
        private Side _side = Side.White;
        private IGameBoard _board;
        private readonly IDropResolver _dropSelector;

        public AIGamePlayer(IDropResolver dropSelector)
        {
            _dropSelector = dropSelector;
        }

        public IGameBoard Board
        {
            get { return _board; }
            set
            {
                if (_board != null)
                    throw new InvalidOperationException();
                if (value == null)
                    throw new ArgumentNullException("value");
                _board = value;
                _board.PieceDropped += OnBoardPieceDropped;
                OnPlaygroundChanged();
            }
        }

        public Side Side
        {
            get { return _side; }
            set
            {
                _side = value;
                OnPlaygroundChanged();
            }
        }

        protected virtual void OnPlaygroundChanged()
        {
            if (Side == Side.Black && _board.DropsCount == 0)
            {
                _board.Drop(new BoardPosition(7, 7), OperatorType.AI);
            }
        }

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            if (Board.ExpectedNextTurn == Side && e.OperatorType == OperatorType.Human)
            {
                Task.Factory.StartNew(() =>
                {
                    var drops = _dropSelector.Resolve(Board, Side);
                    Board.Drop(drops.First().Position, OperatorType.AI);
                });
            }
        }
    }
}
