namespace Renju.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Model;

    [Serializable]
    public class GameBoard : VirtualGameBoard<BoardPoint>, IGameBoard<BoardPoint>, IDisposable
    {
        [NonSerialized]
        private readonly IDisposable _optionsObserver;

        private Side? _expectedNextTurn = Side.Black;

        public GameBoard(NewGameOptions newGameOptions, GameOptions options)
            : base(newGameOptions.BoardSize, BoardPoint.CreateIndexBasedFactory(newGameOptions.BoardSize))
        {
            Options = newGameOptions;
            RuleEngine = newGameOptions.RuleEngine;
            _optionsObserver = options.ObserveProperty(() => options.ShowLinesOnBoard)
                                      .Subscribe(_ => OnPropertyChanged(() => Lines));
        }

        public NewGameOptions Options { get; private set; }

        public Side? ExpectedNextTurn
        {
            get { return _expectedNextTurn; }
        }

        public void BeginGame()
        {
            RaiseGameBeginEvent(Options);
        }

        public DropResult Drop(BoardPosition position, OperatorType type)
        {
            if (_expectedNextTurn.HasValue && this[position].Status == null)
                return Put(new PieceDrop(position, _expectedNextTurn.Value), type);
            else
                return DropResult.InvalidDrop;
        }

        public virtual void Dispose()
        {
            _optionsObserver.Dispose();
        }

        public void Take(BoardPosition position)
        {
            var point = GetPoint(position);
            Debug.Assert(point.Status != null, String.Format("{0} hasn't been dropped.", position));
            var lastPoint = DroppedPoints.Last();
            Debug.Assert(Equals(point, lastPoint), String.Format("{0} wasn't the last drop.", position));
            _expectedNextTurn = point.Status.Value;
            point.ResetToEmpty();
            RaisePieceTakenEvent(position);
        }

        protected virtual DropResult Put(PieceDrop drop, OperatorType type)
        {
            var forbiddenRule = RuleEngine.GetRuleStopDropOn(this, drop);
            if (forbiddenRule != null)
                throw new InvalidOperationException(String.Format("Can't drop on {0} according to rule \"{1}\"", drop, forbiddenRule.Name));
            SetState(drop, drop.Side);
            var hasWon = RuleEngine.IsWin(this, drop);
            var result = hasWon.HasValue ? DropResult.Win(drop.Side) : DropResult.NoWin(RuleEngine.GetNextSide(this, drop));
            _expectedNextTurn = result.ExpectedNextSide;
            RaisePeiceDroppedEvent(drop, type);

            return result;
        }

        protected void SetState(BoardPosition position, Side side)
        {
            var point = GetPoint(position);
            point.Status = side;
            point.Index = DroppedPoints.Count() + 1;
        }
    }
}
