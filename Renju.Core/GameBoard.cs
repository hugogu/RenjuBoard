﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Renju.Core
{
    public class GameBoard
    {
        private List<BoardPoint> _points;
        private List<PieceDrop> _drops = new List<PieceDrop>();
        private IGameRuleEngine _gameRuleEngine;
        private Side? _expectedNextTurn = Side.Black;

        public GameBoard(int size, IGameRuleEngine gameRuleEngine)
        {
            Size = size;
            _gameRuleEngine = gameRuleEngine;
            _points = new List<BoardPoint>(Enumerable.Range(0, size * size).Select(i => CreateBoardPoint(PositionOfIndex(i))));
        }

        public int Size { get; private set; }

        public IReadOnlyCollection<BoardPoint> Points
        {
            get { return new ReadOnlyCollection<BoardPoint>(_points); }
        }

        public IReadOnlyCollection<PieceDrop> Drops
        {
            get { return _drops; }
        }

        public BoardPoint this[int x, int y]
        {
            get { return _points[y * Size + x]; }
        }

        public BoardPoint this[BoardPosition position]
        {
            get { return _points[position.Y * Size + position.X]; }
        }

        public DropResult Drop(BoardPoint point)
        {
            if (_expectedNextTurn.HasValue)
                return Put(new PieceDrop(point.Position.X, point.Position.Y, _expectedNextTurn.Value));
            else
                throw new InvalidOperationException();
        }

        public void UndoLastDrop()
        {
            Take(Drops.Last());
        }

        public void Take(PieceDrop drop)
        {
            if (_drops.Remove(drop))
            {
                var point = this[drop.X, drop.Y];
                _expectedNextTurn = point.Status.Value;
                point.Status = null;
                point.Index = null;
            }
        }

        public DropResult Put(PieceDrop drop)
        {
            var result = _gameRuleEngine.ProcessDrop(this, drop);
            if (result != DropResult.InvalidDrop)
            {
                _drops.Add(drop);
                _expectedNextTurn = result.ExpectedNextSide;
            }

            return result;
        }

        protected virtual BoardPoint CreateBoardPoint(BoardPosition position)
        {
            return new BoardPoint(position);
        }

        private BoardPosition PositionOfIndex(int index)
        {
            return new BoardPosition(index % Size, index / Size);
        }
    }
}
