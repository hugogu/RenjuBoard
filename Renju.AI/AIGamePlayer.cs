using System;
using System.Linq;
using System.Threading.Tasks;
using Renju.Core;

namespace Renju.AI
{
    public class AIGamePlayer : IGamePlayer
    {
        private GameBoard _board;
        private IDropResolver _dropSelector;

        public AIGamePlayer(IDropResolver dropSelector)
        {
            _dropSelector = dropSelector;
        }

        public GameBoard Board
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
            }
        }

        public Side Side { get; set; }

        private void OnBoardPieceDropped(object sender, PieceDropEventArgs e)
        {
            if (Board.ExpectedNextTurn == Side)
            {
                Task.Factory.StartNew(() =>
                {
                    var drops = _dropSelector.Resolve(Board, Side);
                    Board.Drop(drops.First().Position);
                });
            }
        }
    }
}
